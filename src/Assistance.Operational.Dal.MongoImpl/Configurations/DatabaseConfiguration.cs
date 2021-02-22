using Sogetrel.Sinapse.Framework.Dal.MongoDb;

namespace Assistance.Operational.Dal.MongoImpl.Configurations
{
    ///<inheritdoc cref="IMongoDbConfiguration"/>
    public class DatabaseConfiguration : IMongoDbConfiguration
    {
        public string ActivitiesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
