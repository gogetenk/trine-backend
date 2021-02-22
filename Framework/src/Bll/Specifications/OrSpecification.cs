﻿using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public class OrSpecification<T> : ComposeSpecification<T>
    {
        public OrSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right)
        {

        }

        public override bool IsSatisfiedBy(T input)
        {
            var result = _left.IsSatisfiedBy(input) || _right.IsSatisfiedBy(input);
            return result;
        }

        public override SpecificationException GetErrors(T input)
        {
            return null;
        }
    }
}