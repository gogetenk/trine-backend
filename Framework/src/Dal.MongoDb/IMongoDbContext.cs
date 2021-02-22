using MongoDB.Driver;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb
{
    public interface IMongoDbContext
    {
        IMongoDatabase GetDatabase(string databaseName = null);
    }
}
