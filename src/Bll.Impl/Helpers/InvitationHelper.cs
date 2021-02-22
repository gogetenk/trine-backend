using System;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public class InvitationHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IInviteRepository _inviteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _OrganizationDtoRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IMapper _mapper;

        public InvitationHelper(IMapper mapper,
            IConfiguration configuration,
            IInviteRepository inviteRepository,
            IUserRepository userRepository,
            IOrganizationRepository OrganizationDtoRepository,
            IMailRepository mailRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _inviteRepository = inviteRepository;
            _userRepository = userRepository;
            _OrganizationDtoRepository = OrganizationDtoRepository;
            _mailRepository = mailRepository;
        }

        private async Task SendMailToInviter(InviteDto invitation, OrganizationMemberDto guest)
        {
            if (string.IsNullOrEmpty(invitation.InviterId))
                throw new BusinessException("The inviter id cannot be null");

            var inviter = await _userRepository.ApiUsersByIdGetAsync(invitation.InviterId);
            var inviterDto = _mapper.Map<UserDto>(inviter);
            var templateId = _configuration["Mail:Templates:AcceptOrganizationDtoInvitation"];
            var dynamicTemplateData = new
            {
                guest = new
                {
                    firstName = guest.Firstname,
                    lastName = guest.Lastname
                },
                user = new
                {
                    firstName = inviterDto.Firstname,
                    lastName = inviterDto.Lastname
                },
                OrganizationDtoName = invitation.OrganizationName
            };
            await _mailRepository.SendAsync(inviter.Email, templateId: templateId, templateData: dynamicTemplateData);
        }

        private async Task<OrganizationMemberDto> CreateMemberToOrganizationDto(string userId, string OrganizationDtoId, OrganizationMemberDto member)
        {
            var createdMember = await _OrganizationDtoRepository.ApiOrganizationsByOrganizationIdMembersPutAsync(OrganizationDtoId, member);
            if (createdMember == null)
            {
                throw new TechnicalException($"An error occured while creating the member : [{userId}] in OrganizationDto : [{OrganizationDtoId}]");
            }
            return _mapper.Map<OrganizationMemberDto>(createdMember);
        }

        public async Task<InviteDto> VerifyInvitation(string code)
        {
            Guid.TryParse(code, out Guid invitationCode);
            var invite = await _inviteRepository.ApiInvitesByCodeGetAsync(invitationCode);
            if (invite is null)
            {
                throw new BusinessException(ErrorMessages.invitationDoesntExist);
            }

            var inviteDto = _mapper.Map<InviteDto>(invite);
            if (inviteDto.CurrentStatus == InviteDto.Status.Rejected || inviteDto.CurrentStatus == InviteDto.Status.Canceled)
            {
                throw new BusinessException(ErrorMessages.inviteRevoked);
            }
            else if (inviteDto.Expires <= DateTime.UtcNow)
            {
                throw new BusinessException(ErrorMessages.inviteExpired);
            }

            return inviteDto;
        }

        public async Task<AcceptInvitationResultDto> AcceptInvitation(string code)
        {
            var inviteDto = await VerifyInvitation(code);
            var userMatches = await _userRepository.ApiUsersSearchGetAsync(inviteDto.GuestEmail);
            if (userMatches is null || !userMatches.Any())
            {
                throw new BusinessException(ErrorMessages.userDoesntExist);
            }

            var user = userMatches[0];
            var organizationMemberDto = _mapper.Map<OrganizationMemberDto>(new OrganizationMemberDto()
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                JoinedAt = DateTime.UtcNow,
                Icon = user.ProfilePicUrl,
                Role = OrganizationMemberDto.RoleEnum.GUEST,
                UserId = user.Id
            });

            var insertedMember = await CreateMemberToOrganizationDto(user.Id, inviteDto.OrganizationId, organizationMemberDto);
            var memberDto = _mapper.Map<OrganizationMemberDto>(insertedMember);
            await SendMailToInviter(inviteDto, memberDto);

            return new AcceptInvitationResultDto
            {
                JoinedOrganizationId = inviteDto.OrganizationId,
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}
