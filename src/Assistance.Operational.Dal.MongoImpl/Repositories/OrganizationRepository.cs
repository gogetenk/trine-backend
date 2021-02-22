using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Dto;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.MongoImpl.Repositories
{
    public class OrganizationRepository : Impl.Repositories.CrudRepositoryBase<Organization>, IOrganizationRepository
    {
        protected override string CollectionName => "organizations";

        public OrganizationRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper)
            : base(mongoDbContext, logger, mapper)
        {
        }

        public async Task<OrganizationDto> ApiOrganizationsByIdGetAsync(string id, string apiVersion = null)
        {
            return Mapper.Map<OrganizationDto>(GetById(id));
        }

        public async Task ApiOrganizationsByOrganizationIdDeleteAsync(string organizationId, string apiVersion = null)
        {
            Delete(organizationId);
        }

        public async Task<List<InviteDto>> ApiOrganizationsByOrganizationIdInvitesGetAsync(string organizationId, string apiVersion = null)
        {
            var collection = GetCollection<InviteDto>();
            var results = collection
                .AsQueryable()
                .Where(x => x.OrganizationId == organizationId)
                .ToList();

            return Mapper.Map<List<InviteDto>>(results);
        }

        public OrganizationDto GetOrganization(string userId)
        {
            var organizations = FindBy(o => o.OwnerId == userId || o.Members.Any(m => m.UserId == userId && m.Role > OrganizationMember.RoleEnum.MEMBER));
            return Mapper.Map<OrganizationDto>(organizations.FirstOrDefault());

        }

        public async Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdDeleteAsync(string organizationId, string userId, string apiVersion = null)
        {
            var orga = GetById(organizationId);
            orga.Members.RemoveAll(x => x.UserId == userId);
            var isSuccess = Update(organizationId, Mapper.Map<Organization>(orga));

            if (!isSuccess)
                throw new BusinessException("An error occured while removing the member from the organization");
            if (!ObjectId.TryParse(organizationId, out ObjectId objectId))
                throw new DalException("The object id is not well formated");

            return Mapper.Map<OrganizationMemberDto>(FindBy(x => x.Id == objectId && x.Members.Any(y => y.UserId == userId))?.FirstOrDefault()?.Members?.FirstOrDefault(x => x.UserId == userId));
        }

        public async Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdGetAsync(string organizationId, string userId, string apiVersion = null)
        {
            if (!ObjectId.TryParse(organizationId, out ObjectId objectId))
                throw new DalException("The object id is not well formated");

            return Mapper.Map<OrganizationMemberDto>(FindBy(x => x.Id == objectId && x.Members.Any(y => y.UserId == userId))?.FirstOrDefault()?.Members?.FirstOrDefault(x => x.UserId == userId));
        }

        public async Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersByUserIdPatchAsync(string organizationId, string userId, OrganizationMemberDto member = null, string apiVersion = null)
        {
            var orga = GetById(organizationId);
            if (orga.Members is null)
                return null;

            var oldMember = orga.Members.FirstOrDefault(x => x.UserId == member.UserId);
            if (oldMember is null)
                return null;

            orga.Members.Remove(oldMember);
            orga.Members.Add(Mapper.Map<OrganizationMember>(member));
            Update(organizationId, orga);
            return member;
        }

        public async Task<int> ApiOrganizationsByOrganizationIdMembersCountGetAsync(string organizationId, string apiVersion = null)
        {
            if (!ObjectId.TryParse(organizationId, out ObjectId id))
                return 0;

            var collection = GetCollection<Entities.Organization>();
            return collection.AsQueryable().Where(o => o.Id == id).Select(o => o.Members).Count();
        }

        public async Task<List<OrganizationMemberDto>> ApiOrganizationsByOrganizationIdMembersGetAsync(string organizationId, string apiVersion = null)
        {
            return Mapper.Map<List<OrganizationMemberDto>>(GetById(organizationId).Members);
        }

        public async Task<OrganizationMemberDto> ApiOrganizationsByOrganizationIdMembersPutAsync(string organizationId, OrganizationMemberDto member = null, string apiVersion = null)
        {
            var orga = GetById(organizationId);
            if (orga is null)
                throw new BusinessException("The organization doesn't exist");

            if (orga.Members is null)
                orga.Members = new List<OrganizationMember>();

            orga.Members.Add(Mapper.Map<OrganizationMember>(member));
            Update(organizationId, orga);
            return member;
        }

        public async Task<OrganizationDto> ApiOrganizationsByOrganizationIdPatchAsync(string organizationId, OrganizationDto OrganizationDto = null, string apiVersion = null)
        {
            var collection = MongoDatabase.GetCollection<BsonDocument>(CollectionName);
            if (!ObjectId.TryParse(organizationId, out ObjectId castedId))
                return null;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", castedId);

            var changesDocument = BsonDocument.Parse(JsonConvert.SerializeObject(OrganizationDto, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            UpdateDefinition<BsonDocument> update = null;

            foreach (var change in changesDocument)
            {
                if (change.Name == "Id" || change.Value == null || change.Value == default(ObjectId) || change.Value == BsonNull.Value || change.Value.AsString.Contains("0001-01-01"))
                    continue;

                if (update == null)
                {
                    var builder = Builders<BsonDocument>.Update;
                    update = builder.Set(change.Name, change.Value);
                }
                else
                {
                    update = update.Set(change.Name, change.Value);
                }
            }
            var registry = BsonSerializer.SerializerRegistry;
            var serializer = registry.GetSerializer<BsonDocument>();
            var rendered = update.Render(serializer, registry).ToJson();

            update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument("$set", changesDocument));
            var result = collection.UpdateOne(filter, rendered);

            if (!result.IsAcknowledged || result.ModifiedCount == 0)
                throw new DalException("An error occured while updating the organization");

            return Mapper.Map<OrganizationDto>(GetById(organizationId));
        }

        public async Task<List<OrganizationDto>> ApiOrganizationsGetAsync(string apiVersion = null)
        {
            return Mapper.Map<List<OrganizationDto>>(FindBy(x => true));
        }

        public async Task<List<OrganizationDto>> ApiOrganizationsManyGetAsync(List<string> ids = null, string apiVersion = null)
        {
            return Mapper.Map<List<OrganizationDto>>(FindBy(x => ids.Contains(x.Id.ToString())));
        }

        public async Task<List<PartialOrganizationDto>> ApiOrganizationsMembersByUserIdGetAsync(string userId, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Argument null", nameof(userId));

            var orgas = FindBy(x => x.Members.Any(y => y.UserId == userId)).ToList();

            List<PartialOrganizationDto> partialOrgas = orgas.Select(x => new PartialOrganizationDto
            {
                Id = x.Id.ToString(),
                Icon = x.Icon,
                IsOwner = x.OwnerId == userId,
                Name = x.Name,
                UserRole = (OrganizationMemberDto.RoleEnum)x.Members.FirstOrDefault(y => y.UserId == userId).Role,
                MembersNb = x.Members.Count()
            }).ToList();

            return partialOrgas;
        }

        public async Task<OrganizationDto> ApiOrganizationsPostAsync(OrganizationDto organizationDto = null, string apiVersion = null)
        {
            var orga = Insert(Mapper.Map<Organization>(organizationDto));
            return Mapper.Map<OrganizationDto>(orga);
        }

        public async Task<OrganizationDto> ApiOrganizationsPutAsync(string id = null, OrganizationDto organization = null, string apiVersion = null)
        {
            return Mapper.Map<OrganizationDto>(organization);
        }


    }
}
