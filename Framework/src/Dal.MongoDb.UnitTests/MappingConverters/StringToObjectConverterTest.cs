using AutoMapper;
using FluentAssertions;
using MongoDB.Bson;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.MappingConverters;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Dal.MongoDb.UnitTests.MappingConverters
{
    [Trait("Category", "UnitTests")]
    public class StringToObjectConverterTest
    {
        private readonly IMapper _mapper;

        public StringToObjectConverterTest()
        {
            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<string, ObjectId>()
                    .ConvertUsing<StringToObjectIdConverter>();
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
        public void Map_WhenValidStringInput_ThenFilledObjectIdReturned()
        {
            var output = _mapper.Map<ObjectId>("5afc4e15b990b8062ddabe1b");
            output.ToString().Should().Be("5afc4e15b990b8062ddabe1b");
            output.CreationTime.ToString("yyyy-MM-dd HH-mm-ss").Should().Be("2018-05-16 15-28-21");
            output.Machine.Should().Be(12161208);
        }

        [Fact]
        public void Map_WhenNullInput_ThenEmptyObjectIdReturned()
        {
            var output = _mapper.Map<ObjectId>(null);
            (output == ObjectId.Empty).Should().BeTrue();
            output.ToString().Should().Be("000000000000000000000000");
        }

        [Fact]
        public void Map_WhenEmptyStringInput_ThenEmptyObjectIdReturned()
        {
            var output = _mapper.Map<ObjectId>(string.Empty);
            (output == ObjectId.Empty).Should().BeTrue();
            output.ToString().Should().Be("000000000000000000000000");
        }
    }
}
