using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Sogetrel.Sinapse.Framework.SerializationExtensions
{
    /// <summary>
    /// Extension class providing JSON-related methods
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts an object to a json string
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>Json string</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Converts a json string to an object
        /// </summary>
        /// <typeparam name="T">Object type to convert to</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>Object converted</returns>
        public static T DeserializeFromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Tries to convert a json string to an object
        /// </summary>
        /// <typeparam name="T">Object type to convert to</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>Object converted or default object</returns>
        public static bool TryParseJson<T>(this string json, out T output)
        {
            var schemaGenerator = new JSchemaGenerator();
            var schema = schemaGenerator.Generate(typeof(T));
            var jObject = JObject.Parse(json);

            if (jObject.IsValid(schema))
            {
                output = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            else
            {
                output = default(T);
                return false;
            }
        }
    }
}
