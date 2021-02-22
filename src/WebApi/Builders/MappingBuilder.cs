using Assistance.Operational.WebApi.MappingProfiles;
using AutoMapper;

namespace Assistance.Operational.WebApi.Builders
{
    public class MappingBuilder
    {
        /// <summary>
        /// Build mapping configuration.
        /// </summary>
        /// <returns></returns>
        public IMapper BuildAutoMapper()
        {
            var config = new MapperConfiguration(x =>
            {
                x.AddProfile(new MappingProfile());
                x.AllowNullCollections = true;
            });

            var mapper = config.CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            return mapper;
        }
    }
}
