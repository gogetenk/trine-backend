namespace Sogetrel.Sinapse.Framework.Dal.MongoDb
{
    public interface IMongoDbConfiguration
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}
