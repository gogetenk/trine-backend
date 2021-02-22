using AutoMapper;
using MongoDB.Bson;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb.MappingConverters
{
    public class ObjectIdToStringConverter : ITypeConverter<ObjectId, string>
    {
        public string Convert(ObjectId source, string destination, ResolutionContext context)
        {
            if (source == ObjectId.Empty)
                return null;
            return source.ToString();
        }
    }
}
