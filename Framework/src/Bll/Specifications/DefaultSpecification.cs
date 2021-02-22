using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public class DefaultSpecification<T> : SpecificationBase<T>
    {
        public override bool IsSatisfiedBy(T input)
        {
            return true;
        }

        public override SpecificationException GetErrors(T input)
        {
            return null;
        }

    }

}
