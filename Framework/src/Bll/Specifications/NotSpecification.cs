using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public class NotSpecification<T> : ComposeSpecification<T>
    {
        public NotSpecification(ISpecification<T> left) : base(left, null)
        {
        }

        public override bool IsSatisfiedBy(T input)
        {
            var result = !_left.IsSatisfiedBy(input);
            return result;
        }

        public override SpecificationException GetErrors(T input)
        {

            return _left.IsSatisfiedBy(input) ? _left.GetErrors(input) : null;
        }
    }
}
