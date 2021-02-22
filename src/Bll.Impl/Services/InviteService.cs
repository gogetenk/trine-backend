using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Helpers;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class InviteService : ServiceBase, IInviteService
    {
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _OrganizationDtoRepository;
        private readonly IInviteRepository _inviteRepository;
        private readonly IConfiguration _configuration;
        private readonly IMailRepository _mailRepository;
        private readonly IUserRepository _userRepository;

        public InviteService(ILogger<ServiceBase> logger, IMapper mapper, IOrganizationRepository OrganizationDtoRepository,
            IInviteRepository inviteRepository, IConfiguration configuration, IMailRepository mailRepository, IUserRepository userRepository) : base(logger)
        {
            _mapper = mapper;
            _OrganizationDtoRepository = OrganizationDtoRepository;
            _inviteRepository = inviteRepository;
            _configuration = configuration;
            _mailRepository = mailRepository;
            _userRepository = userRepository;
        }

        public async Task<InviteDto[]> CreateBulkInvite(string[] mails, string inviterId, string OrganizationDtoId, string missionId = "")
        {
            var tasks = new List<Task<InviteDto>>();
            foreach (var mail in mails)
            {
                tasks.Add(CreateInvite(mail, inviterId, OrganizationDtoId, missionId));
            }
            var sucessfullInvitations = await Task.WhenAll(tasks);
            return sucessfullInvitations;
        }

        public async Task<InviteDto> CreateInvite(string mail, string inviterId, string OrganizationDtoId, string missionId = "")
        {
            if (string.IsNullOrEmpty(mail))
                throw new BusinessException("Required mail");

            var orga = await _OrganizationDtoRepository.ApiOrganizationsByIdGetAsync(OrganizationDtoId);
            if (orga is null)
                throw new BusinessException("The OrganizationDto no longer exists.");

            if (orga.Members is null)
                orga.Members = new List<OrganizationMemberDto>();

            var inviter = orga.Members.FirstOrDefault(y => y.UserId == inviterId);
            if (inviter is null)
                throw new BusinessException("The user that created this invite no longer exists in this OrganizationDto.");

            //var role = _mapper.Map<RoleEnum>(inviter.Role);
            //if (role < RoleEnum.MANAGER)
            //    throw new BusinessException("The user that created this invite has no longer the sufficient rights to invite you.");

            var invite = new InviteDto()
            {
                Code = Guid.NewGuid(),
                InviterId = inviterId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(5),
                CurrentStatus = InviteDto.Status.Pending,
                OrganizationId = orga.Id,
                OrganizationName = orga.Name,
                MissionId = missionId,
                GuestEmail = mail
            };

            var link = $"{_configuration["WebApp:Url"]}{_configuration["WebApp:JoinPath"]}?code={invite.Code}";
            var newInvite = await _inviteRepository.ApiInvitesPostAsync(_mapper.Map<InviteDto>(invite));
            var invitationDto = _mapper.Map<InviteDto>(newInvite);
            if (invitationDto is null)
                throw new TechnicalException("An error occured after creating the invitation.");

            var guest = await _userRepository.ApiUsersByIdGetAsync(inviterId);
            var templateId = _configuration["Mail:Templates:OrganizationDtoInvitation"];
            if (templateId != null)
            {
                var dynamicTemplateData = new
                {
                    link,
                    guest = new
                    {
                        firstName = guest.Firstname,
                        lastName = guest.Lastname,
                    },
                    OrganizationDto = new
                    {
                        icon = orga.Icon,
                        name = orga.Name
                    }
                };
                await _mailRepository.SendAsync(mail, templateId: templateId, templateData: dynamicTemplateData);
            }
            return invitationDto;
        }

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<InviteDto>> GetUserInvitesAsync(string id)
        {
            var user = await _userRepository.ApiUsersByIdGetAsync(id);
            if (user == null)
                throw new BusinessException("The user does not exist.");

            var invites = await _inviteRepository.ApiInvitesEmailByEmailGetAsync(user.Email);
            return _mapper.Map<List<InviteDto>>(invites);
        }

        public async Task<InviteResponseDto> Get(Guid code)
        {
            var invite = await _inviteRepository.ApiInvitesByCodeGetAsync(code);
            var inviteDto = _mapper.Map<InviteDto>(invite);
            var users = await _userRepository.ApiUsersSearchGetAsync(inviteDto.GuestEmail);
            return new InviteResponseDto()
            {
                Created = inviteDto.Created,
                Expires = inviteDto.Expires,
                GuestEmail = inviteDto.GuestEmail,
                InviterId = inviteDto.InviterId,
                MissionId = inviteDto.MissionId,
                OrganizationId = inviteDto.OrganizationId,
                OrganizationName = inviteDto.OrganizationName,
                Code = inviteDto.Code,
                Id = invite.Id,
                UnknownUser = !users.Any(),
                CurrentStatus = inviteDto.CurrentStatus
            };
        }

        public async Task<AcceptInvitationResultDto> AcceptInvite(string code)
        {
            var helper = new InvitationHelper(_mapper, _configuration, _inviteRepository, _userRepository, _OrganizationDtoRepository, _mailRepository);
            var user = await helper.AcceptInvitation(code);
            return _mapper.Map<AcceptInvitationResultDto>(user);
        }

        public async Task<List<InviteDto>> GetByOrganizationId(string OrganizationDtoId)
        {
            var invitations = await _inviteRepository.ApiInvitesSearchGetAsync(OrganizationDtoId);
            return _mapper.Map<List<InviteDto>>(invitations);
        }
    }
}
