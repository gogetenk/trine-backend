using System;
using FluentAssertions;
using Sogetrel.Sinapse.Framework.Bll.Specifications;
using Sogetrel.Sinapse.Framework.Exceptions;
using Xunit;

namespace Sogetrel.Sinapse.Framework.Bll.UnitTests.Specifications
{
    [Trait("Category", "UnitTests")]
    public class SpecificationBaseTests
    {
        #region Internal classes definition

        public class DummySpec : ISpecification<bool>
        {
            public SpecificationException GetErrors(bool input)
            {
                throw new NotImplementedException();
            }

            public bool IsSatisfiedBy(bool input)
            {
                return input;
            }
        }

        public class DummySpecificationBase : SpecificationBase<DummySpec>
        {
            private readonly bool _input;

            public DummySpecificationBase(bool input)
            {
                _input = input;
            }

            public override SpecificationException GetErrors(DummySpec input)
            {
                return new SpecificationException("1", "description", "cause");
            }

            public override bool IsSatisfiedBy(DummySpec input)
            {
                return input.IsSatisfiedBy(_input);
            }
        }

        #endregion

        #region Unit Tests

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        public void AndSpecification_NominalCase(bool left, bool right, bool result)
        {

            //Act           
            var specLeft = new DummySpecificationBase(left);
            var dummySpec = new DummySpec();
            var specRight = new DummySpecificationBase(right);

            //Arrange
            var andSpecification = specLeft.And(specRight);

            //Assert
            andSpecification.IsSatisfiedBy(dummySpec).Should().Be(result);

        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public void OrSpecification_NominalCase(bool left, bool right, bool result)
        {

            //Act           
            var specLeft = new DummySpecificationBase(left);
            var dummySpec = new DummySpec();
            var specRight = new DummySpecificationBase(right);

            //Arrange
            var orSpecification = specLeft.Or(specRight);
            //Assert
            orSpecification.IsSatisfiedBy(dummySpec).Should().Be(result);
        }

        [Fact]
        public void NotSpecification_NominalCase()
        {
            //Act 
            var spec = new DummySpecificationBase(true);
            var dummySpec = new DummySpec();

            //Arrange
            var notSpecification = spec.Not();

            //Assert
            notSpecification.IsSatisfiedBy(dummySpec).Should().Be(false);
        }

        #endregion
    }
}
