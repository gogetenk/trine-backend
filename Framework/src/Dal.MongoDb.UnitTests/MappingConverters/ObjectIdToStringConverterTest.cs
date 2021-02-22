using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.MappingConverters;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb.UnitTests.MappingConverters
{
    public class ObjectIdToStringConverterTest
    {
        private readonly IMapper _mapper;

        public ObjectIdToStringConverterTest()
        {
            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<ObjectId, string>()
                    .ConvertUsing<ObjectIdToStringConverter>();
                x.AllowNullCollections = true;
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void CheckMappingConfigurationIsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_WhenFilledObjectIdInput_ThenFilledStringReturned()
        {
            var output = _mapper.Map<string>(new ObjectId("5afc4e15b990b8062ddabe1b"));
            output.Should().Be("5afc4e15b990b8062ddabe1b");
        }

        [Fact]
        public void Map_WhenEmptyObjectIdInput_ThenNullReturned()
        {
            var output = _mapper.Map<string>(ObjectId.Empty);
            output.Should().BeNull();
        }
    }
}
