using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        public SpecificationBase<T> And(ISpecification<T> input)
        {
            return new AndSpecification<T>(this, input);
        }

        public SpecificationBase<T> Or(ISpecification<T> input)
        {
            return new OrSpecification<T>(this, input);
        }

        public SpecificationBase<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        public abstract bool IsSatisfiedBy(T input);

        public abstract SpecificationException GetErrors(T input);
    }
}
