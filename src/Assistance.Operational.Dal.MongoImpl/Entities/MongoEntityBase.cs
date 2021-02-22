using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class MongoEntityBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
