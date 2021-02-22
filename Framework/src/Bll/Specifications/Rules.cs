using System;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Sogetrel.Sinapse.Framework.Bll.Specifications
{
    public class Rules<T> : SpecificationBase<T>
    {
        private SpecificationBase<T> _rules = new DefaultSpecification<T>();

        public static Rules<T> New
        {
            get
            {
                return new Rules<T>();
            }
        }

        public Rules<T> And<TCustom>() where TCustom : ISpecification<T>
        {
            var rule = (TCustom)Activator.CreateInstance(typeof(TCustom));
            _rules = _rules.And(rule);
            return this;
        }

        public Rules<T> Or<TCustom>() where TCustom : ISpecification<T>
        {
            var rule = (TCustom)Activator.CreateInstance(typeof(TCustom));
            _rules = _rules.Or(rule);
            return this;
        }

        public Rules<T> AndNot<TCustom>() where TCustom : SpecificationBase<T>
        {
            var rule = (SpecificationBase<T>)Activator.CreateInstance(typeof(TCustom));
            rule = rule.Not();
            _rules = _rules.And(rule);
            return this;
        }

        public override bool IsSatisfiedBy(T input)
        {
            return _rules.IsSatisfiedBy(input);
        }

        public override SpecificationException GetErrors(T input)
        {
            return _rules.IsSatisfiedBy(input) ? null : _rules.GetErrors(input);
        }
    }
}
