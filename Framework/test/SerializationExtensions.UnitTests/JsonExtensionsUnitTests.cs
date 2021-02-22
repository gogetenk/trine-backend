using System;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Sogetrel.Sinapse.Framework.SerializationExtensions.UnitTests
{
    [Trait("Category", "UnitTests")]
    public class JsonExtensionsUnitTests
    {
        [Fact]
        public void ToJson_NominalCase_ExpectsValidOutput()
        {
            // Arrange
            var testObject = new { FirstName = "Obi-Wan", LastName = "Kenobi" };
            var expectedOutput = JsonConvert.SerializeObject(testObject);

            // Act
            var output = testObject.ToJson();

            // Assert
            output.Should().NotBeNull();
            output.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void ToJson_WithDates_ExpectsValidOutput()
        {
            // Arrange
            var date = DateTime.Now;
            var testObject = new { Date = date };
            var expectedOutput = JsonConvert.SerializeObject(testObject);

            // Act
            var output = testObject.ToJson();

            // Assert
            output.Should().NotBeNull();
            output.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void ToJson_WithDatesAndMilisecondsEndingInZero_ExpectsOutputWithTruncatedMiliseconds()
        {
            // Arrange
            var date = new DateTime(1941, 12, 7, 17, 48, 01, 100, DateTimeKind.Utc);
            var testObject = new { Date = date };
            var expectedOutput = JsonConvert.SerializeObject(testObject);

            // Act
            var output = testObject.ToJson();

            // Assert
            output.Should().NotBeNull();
            output.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void DeserializeFromJson_NominalCase_ExpectsValidOutput()
        {
            // Arrange
            var jsonString = "{\"FirstName\": \"Santa\", \"LastName\": \"Klaus\"}";
            var expectedOutput = JsonConvert.DeserializeObject(jsonString);

            // Act
            var output = jsonString.DeserializeFromJson<Dummy>();

            // Assert
            output.Should().NotBeNull();
            output.FirstName.Should().BeEquivalentTo("Santa");
            output.LastName.Should().BeEquivalentTo("Klaus");
        }

        [Fact]
        public void TryParseJson_WithValidJson_ExpectsValidOutput()
        {
            // Arrange
            var jsonString = "{\"FirstName\": \"Jack\", \"LastName\": \"Black\"}";
            var expectedOutput = JsonConvert.DeserializeObject(jsonString);

            // Act
            Dummy output;
            var result = jsonString.TryParseJson<Dummy>(out output);

            // Assert
            result.Should().Be(true);
            output.Should().NotBeNull();
            output.FirstName.Should().BeEquivalentTo("Jack");
            output.LastName.Should().BeEquivalentTo("Black");
        }

        [Fact]
        public void TryParseJson_WithInvalidJson_ExpectsDefaultOutput()
        {
            // Arrange
            var jsonString = "{\"Nickname\": \"KG\", \"Guitar\": \"Gibson\"}";
            var expectedOutput = JsonConvert.DeserializeObject(jsonString);

            // Act
            Dummy output;
            var result = jsonString.TryParseJson<Dummy>(out output);

            // Assert
            result.Should().Be(false);
            output.Should().Be(default(Dummy));
        }

        [Fact]
        public void TryParseJson_WithPartiallyValidJson_ExpectsDefaultOutput()
        {
            // Arrange
            var jsonString = "{\"Firstname\": \"Mario\", \"Guitar\": \"Gibson\"}";
            var expectedOutput = JsonConvert.DeserializeObject(jsonString);

            // Act
            Dummy output;
            var result = jsonString.TryParseJson<Dummy>(out output);

            // Assert
            result.Should().Be(false);
            output.Should().Be(default(Dummy));
        }

        /// <summary>
        /// Private class used for serialization testing
        /// </summary>
        private class Dummy
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
