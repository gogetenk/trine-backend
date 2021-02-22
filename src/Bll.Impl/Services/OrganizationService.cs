using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Impl.Errors;
using Assistance.Operational.Bll.Services;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class OrganizationService : ServiceBase, IOrganizationService
    {
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;

        private const string _OrganizationDtoApiVersion = "1.0";

        public OrganizationService(ILogger<ServiceBase> logger, IMapper mapper, IOrganizationRepository organizationsRepository, IUserRepository userRepository) : base(logger)
        {
            _mapper = mapper;
            _organizationRepository = organizationsRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationDto> Insert(OrganizationDto OrganizationDto)
        {
            if (string.IsNullOrEmpty(OrganizationDto.Name))
                throw new BusinessException(ErrorMessages.createOrgaNoNameError);

            var createdOrga = await _organizationRepository.ApiOrganizationsPostAsync(_mapper.Map<OrganizationDto>(OrganizationDto), _OrganizationDtoApiVersion);
            if (createdOrga is null)
                throw new BusinessException("An error occured while creating the OrganizationDto. Try again later.");

            return _mapper.Map<OrganizationDto>(createdOrga);
        }

        public async Task<OrganizationDto> GetById(string id)
        {
            var orga = await _organizationRepository.ApiOrganizationsByIdGetAsync(id, _OrganizationDtoApiVersion);
            var dto = _mapper.Map<OrganizationDto>(orga);
            return dto;
        }

        public OrganizationDto GetByUserId(string userId)
        {
            var organization = _organizationRepository.GetOrganization(userId);
            return _mapper.Map<OrganizationDto>(organization);
        }

        public async Task<List<OrganizationDto>> GetByIds(List<string> ids)
        {
            var orgas = await _organizationRepository.ApiOrganizationsManyGetAsync(ids, _OrganizationDtoApiVersion);
            return _mapper.Map<List<OrganizationDto>>(orgas);
        }

        public async Task<OrganizationDto> Update(string id, OrganizationDto OrganizationDto)
        {
            var result = await _organizationRepository.ApiOrganizationsPutAsync(id, _mapper.Map<OrganizationDto>(OrganizationDto));
            return _mapper.Map<OrganizationDto>(result);
        }

        public async Task Delete(string id)
        {
            await _organizationRepository.ApiOrganizationsByOrganizationIdDeleteAsync(id);
        }

        public async Task<OrganizationMemberDto> UpdateMember(string OrganizationDtoId, OrganizationMemberDto member)
        {
            var entity = _mapper.Map<OrganizationMemberDto>(member);
            var result = await _organizationRepository.ApiOrganizationsByOrganizationIdMembersByUserIdPatchAsync(OrganizationDtoId, member.UserId, entity, _OrganizationDtoApiVersion);

            return _mapper.Map<OrganizationMemberDto>(result);
        }

        public async Task<OrganizationMemberDto> GetMember(string OrganizationDtoId, string userId)
        {
            var member = await _organizationRepository.ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(OrganizationDtoId, userId, _OrganizationDtoApiVersion);
            return _mapper.Map<OrganizationMemberDto>(member);
        }

        public async Task<List<UserDto>> GetAllMembers(string OrganizationDtoId)
        {
            var members = _mapper.Map<List<OrganizationMemberDto>>(await _organizationRepository.ApiOrganizationsByOrganizationIdMembersGetAsync(OrganizationDtoId, _OrganizationDtoApiVersion));
            if (members is null || !members.Any())
                return null;

            var users = _mapper.Map<List<UserDto>>(await _userRepository.ApiUsersIdsGetAsync(members.Select(x => x.UserId).ToList()));
            if (users is null || !users.Any())
                return null;

            // On assigne les rôles
            users.ForEach(x =>
            {
                x.Role = Enum.GetName(typeof(OrganizationMemberDto.RoleEnum), members.FirstOrDefault(y => y.UserId == x.Id).Role);
            });
            return users;
        }

        //public async Task<int> GetMembersCount(string OrganizationDtoId)
        //{
        //    var count = await _OrganizationDtoRepository.ApiOrganizationDtosByOrganizationDtoIdMembersGetCount(OrganizationDtoId, _OrganizationDtoApiVersion);
        //    return count;
        //}

        public async Task RemoveMember(string OrganizationDtoId, string userId)
        {
            await _organizationRepository.ApiOrganizationsByOrganizationIdMembersByUserIdDeleteAsync(OrganizationDtoId, userId);
        }

        public async Task<OrganizationDto> PartialUpdate(string OrganizationDtoId, OrganizationDto OrganizationDto)
        {
            var entity = await _organizationRepository.ApiOrganizationsByOrganizationIdPatchAsync(OrganizationDtoId, _mapper.Map<OrganizationDto>(OrganizationDto));
            return _mapper.Map<OrganizationDto>(entity);
        }
    }
}
