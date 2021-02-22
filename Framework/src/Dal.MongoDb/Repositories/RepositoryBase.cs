using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories
{
    public abstract class RepositoryBase
    {
        protected IMongoDbContext MongoDbContext { get; private set; }
        protected IMongoDatabase MongoDatabase { get; private set; }
        protected ILogger<RepositoryBase> Logger { get; private set; }
        protected IMapper Mapper { get; private set; }

        protected abstract string CollectionName { get; }

        protected RepositoryBase(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper)
        {
            MongoDbContext = mongoDbContext;
            MongoDatabase = MongoDbContext.GetDatabase();
            Logger = logger;
            Mapper = mapper;
        }

        protected IMongoCollection<T> GetCollection<T>()
        {
            return MongoDatabase.GetCollection<T>(CollectionName);
        }
    }
}
