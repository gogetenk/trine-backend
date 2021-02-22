namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public abstract class ComposeSpecification<T> : SpecificationBase<T>
    {
        protected ISpecification<T> _left;
        protected ISpecification<T> _right;

        protected ComposeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }
    }
}
