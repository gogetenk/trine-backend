using Assistance.Operational.Bll.Impl.Errors;
using Dto;
using Sogetrel.Sinapse.Framework.Bll.Specifications;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Specifications
{
    public class ValidateCompanyRegistrationRequestSpecification : SpecificationBase<RegisterCompanyRequestDto>
    {
        private const string _errorCode = "001";
        private readonly string _errorMessage = ErrorMessages.companyBadFormat;
        private const string _errorCause = "Bad format";
        private const int _siretLength = 14;

        public override SpecificationException GetErrors(RegisterCompanyRequestDto input)
        {
            return new SpecificationException(_errorCode, _errorMessage, _errorCause);
        }

        public override bool IsSatisfiedBy(RegisterCompanyRequestDto input)
        {
            return
                    !string.IsNullOrEmpty(input.Rcs) &&
                    !string.IsNullOrEmpty(input.Name) &&
                    !string.IsNullOrEmpty(input.UserId) &&
                    !string.IsNullOrEmpty(input.TvaNumber) &&
                    !string.IsNullOrEmpty(input.Siret) &&
                    input.Address != null;
        }
    }
}
