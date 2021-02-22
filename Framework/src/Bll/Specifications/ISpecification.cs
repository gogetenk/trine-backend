using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T input);
        SpecificationException GetErrors(T input);
    }
}
