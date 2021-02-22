using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;

namespace Assistance.Operational.Dal.MongoImpl
{
    public class DefaultMongoDbContext : IMongoDbContext
    {
        private readonly string _databaseName;

        protected MongoClient MongoClient { get; set; }

        public DefaultMongoDbContext(IMongoDbConfiguration configuration)
        {
            string connectionString = configuration.ConnectionString;
            _databaseName = configuration.DatabaseName;
            RegisterConventions();
            MongoClient = new MongoClient(connectionString);
        }

        public IMongoDatabase GetDatabase(string databaseName = null)
        {
            return MongoClient.GetDatabase(databaseName ?? _databaseName);
        }

        protected virtual void RegisterConventions()
        {
            ConventionPack val = new ConventionPack();
            val.Add(new IgnoreIfNullConvention(true));
            ConventionRegistry.Register("IgnoreNullValues", val, (Type t) => true);
            ConventionPack val2 = new ConventionPack();
            val2.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("CamelCaseElementName", val2, (Type t) => true);
            ConventionPack val3 = new ConventionPack();
            val3.Add(new EnumRepresentationConvention(BsonType.String));
            ConventionRegistry.Register("EnumAsString", val3, (Type t) => true);
            ConventionPack val4 = new ConventionPack();
            val4.Add(new IgnoreExtraElementsConvention(true));
            ConventionRegistry.Register("IgnoreExtraElements", val4, (Type t) => true);
        }
    }
}
