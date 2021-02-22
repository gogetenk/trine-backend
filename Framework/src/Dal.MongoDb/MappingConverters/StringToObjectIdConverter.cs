using AutoMapper;
using MongoDB.Bson;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb.MappingConverters
{
    public class StringToObjectIdConverter : ITypeConverter<string, ObjectId>
    {
        public ObjectId Convert(string source, ObjectId destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source) || !ObjectId.TryParse(source, out ObjectId output))
            {
                return ObjectId.Empty;
            }
            return output;
        }
    }
}
