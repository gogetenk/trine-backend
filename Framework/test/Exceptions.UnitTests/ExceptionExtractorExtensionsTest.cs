using System;
using FluentAssertions;
using Sogetrel.Sinapse.Framework.Exceptions;
using Sogetrel.Sinapse.Framework.Exceptions.Extensions;
using Xunit;

namespace Exceptions.UnitTests
{
    [Trait("Category", "UnitTests")]
    public class ExceptionExtractorExtensionsTest
    {
        [Fact]
        public void TestExtractor_WhenInnerExceptionNull_ReturnNull()
        {
            // 1 - Arrange
            var exception = new Exception("", null);

            // 2 - Act
            var output = exception.Extract<BusinessException>();

            // 3 - Assert
            output.Should().BeNull();
        }

        [Fact]
        public void TestExtractor_WhenExceptionIsBusiness_ReturnException()
        {
            // 1 - Arrange
            var message = "test";
            var exception = new BusinessException(message);

            // 2 - Act
            var output = exception.Extract<BusinessException>();

            // 3 - Assert
            output.Should().NotBeNull();
            output.Message.Should().Be(message);
        }

        [Fact]
        public void TestExtractor_WhenInnerExceptionIsBusiness_ReturnException()
        {
            // 1 - Arrange
            var message = "test";
            var exception = new Exception("", new BusinessException(message));

            // 2 - Act
            var output = exception.Extract<BusinessException>();

            // 3 - Assert
            output.Should().NotBeNull();
            output.Message.Should().Be(message);
        }

        [Fact]
        public void TestExtractor_WhenInnerInnerExceptionIsBusiness_ReturnException()
        {
            // 1 - Arrange
            var message = "test";
            var exception = new Exception("", new Exception("", new BusinessException(message)));

            // 2 - Act
            var output = exception.Extract<BusinessException>();

            // 3 - Assert
            output.Should().NotBeNull();
            output.Message.Should().Be(message);
        }
    }
}
