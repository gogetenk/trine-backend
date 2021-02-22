using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Dal.MongoImpl.Configurations;
using Assistance.Operational.Dal.MongoImpl.Entities;
using Assistance.Operational.Dal.Repositories;
using AutoMapper;
using Dto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public class UserRepository : Impl.Repositories.CrudRepositoryBase<User>, IUserRepository
    {
        private readonly DatabaseConfiguration _config;
        protected override string CollectionName => _config.UsersCollectionName;

        public UserRepository(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper, IOptionsSnapshot<DatabaseConfiguration> config)
            : base(mongoDbContext, logger, mapper)
        {
            _config = config.Value;
        }

        public async Task ApiUsersByIdDeleteAsync(string id, string apiVersion = null)
        {
            Delete(id);
        }

        public async Task<UserDto> ApiUsersByIdGetAsync(string id, string apiVersion = null)
        {
            return Mapper.Map<UserDto>(GetById(id));
        }

        public async Task<List<UserDto>> ApiUsersGetAsync(string apiVersion = null)
        {
            return Mapper.Map<List<UserDto>>(FindBy(x => true));
        }

        public async Task<List<UserDto>> ApiUsersIdsGetAsync(List<string> ids = null, string apiVersion = null)
        {
            return Mapper.Map<List<UserDto>>(GetByIds(ids));
        }

        public async Task<UserDto> ApiUsersPostAsync(UserDto user = null, string apiVersion = null)
        {
            // We may not have a password if the user has already been created by auth0 
            if (!string.IsNullOrEmpty(user.Password))
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Password = passwordHash;
            }

            var createdUser = Insert(Mapper.Map<User>(user));
            if (createdUser is null)
                return null;

            return Mapper.Map<UserDto>(createdUser);
        }

        public async Task ApiUsersPutAsync(string id = null, UserDto user = null, string apiVersion = null)
        {
            if (!Update(id, Mapper.Map<User>(user)))
                throw new DalException("The update didn't succeeed");
        }

        public async Task<List<UserDto>> ApiUsersSearchGetAsync(string email = null, string firstname = null, string lastname = null, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(firstname))
                firstname = "";

            if (string.IsNullOrEmpty(lastname))
                lastname = "";

            if (string.IsNullOrEmpty(email))
                email = "";

            var results = FindBy(x => x.Firstname.ToLower().Contains(firstname.ToLower()) &&
            x.Lastname.ToLower().Contains(lastname.ToLower()) &&
            x.Email.ToLower().Contains(email.ToLower()));

            return Mapper.Map<List<UserDto>>(results);
        }

        public async Task<UserDto> GetByEmail(string mail)
        {
            return Mapper.Map<UserDto>(FindBy(x => x.Email == mail).FirstOrDefault());
        }

        public async Task<UserDto> GetByEmailAndPassword(string mail, string password)
        {
            return Mapper.Map<UserDto>(FindBy(x => x.Email == mail && x.Password == password).FirstOrDefault());
        }

        public override bool Update(string id, User entity)
        {
            var collection = GetCollection<BsonDocument>();
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                throw new DalException("The object id is not well formated");

            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
            var changesDocument = BsonDocument.Parse(JsonConvert.SerializeObject(entity, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            UpdateDefinition<BsonDocument> update = null;
            foreach (var change in changesDocument)
            {
                if (change.Value.IsBoolean || change.Value.IsInt32)
                    continue;

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
            var result = collection.UpdateOne(filter, rendered);
            if (!result.IsAcknowledged || result.ModifiedCount == 0)
                throw new DalException("An error occured while updating the organization");

            var updatedEntity = GetById(id);
            return result.IsAcknowledged && updatedEntity != null;
        }
    }
}
