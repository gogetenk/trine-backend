using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;

namespace Assistance.Operational.Dal.MongoImpl.Repositories
{
    public class InviteRepository : Impl.Repositories.CrudRepositoryBase<Invite>, IInviteRepository
    {
        protected override string CollectionName => "invites";

        public InviteRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper) : base(mongoDbContext, logger, mapper)
        {
        }

        public async Task<InviteDto> ApiInvitesByCodeGetAsync(Guid code, string apiVersion = null)
        {
            return Mapper.Map<InviteDto>(FindBy(x => x.Code == code));
        }

        public async Task<InviteDto> ApiInvitesByIdGetAsync(string id, string apiVersion = null)
        {
            return Mapper.Map<InviteDto>(GetById(id));
        }

        public async Task<List<InviteDto>> ApiInvitesEmailByEmailGetAsync(string email, string apiVersion = null)
        {
            return Mapper.Map<List<InviteDto>>(FindBy(x => x.GuestEmail == email));
        }

        public async Task<List<InviteDto>> ApiInvitesGetAsync(string apiVersion = null)
        {
            return Mapper.Map<List<InviteDto>>(FindBy(x => true));
        }

        public async Task<InviteDto> ApiInvitesPostAsync(InviteDto invite = null, string apiVersion = null)
        {
            return Mapper.Map<InviteDto>(Insert(Mapper.Map<Invite>(invite)));
        }

        public async Task<List<InviteDto>> ApiInvitesSearchGetAsync(string organizationId = null, string apiVersion = null)
        {
            return Mapper.Map<List<InviteDto>>(FindBy(x => x.OrganizationId == organizationId));
        }
    }
}
