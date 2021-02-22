using System;
using FluentAssertions;
using Sogetrel.Sinapse.Framework.Web.Http.Extensions;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Web.Http.UnitTests.Extensions
{
    [Trait("Category", "UnitTests")]
    public class DateTimeExtensionTest
    {
        [Fact]
        public void ToUrlString_WhenBasicDateTime_ReturnsUrlFormattedString()
        {
            // Arrange
            var dateTime = new DateTime(2018, 01, 06);

            // Act
            var output = dateTime.ToUrlString();

            // Assert
            output.Should().NotBeNullOrEmpty();
            output.Should().Be("2018-01-06T00:00:00");
        }
    }
}
