using Assistance.Operational.Bll.Impl.Errors;
using Dto;
using Sogetrel.Sinapse.Framework.Bll.Specifications;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Specifications
{
    public class ValidateUserRegistrationRequestSpecification : SpecificationBase<RegisterUserRequestDto>
    {
        private const string _errorCode = "001";
        private readonly string _errorMessage = ErrorMessages.registerUserBadFormat;
        private const string _errorCause = "Bad format";
        //private const int _siretLength = 14;

        public override SpecificationException GetErrors(RegisterUserRequestDto input)
        {
            return new SpecificationException(_errorCode, _errorMessage, _errorCause);
        }

        public override bool IsSatisfiedBy(RegisterUserRequestDto input)
        {
            return
                    !string.IsNullOrEmpty(input.Email) &&
                    !string.IsNullOrEmpty(input.Firstname) &&
                    !string.IsNullOrEmpty(input.Password) &&
                    !string.IsNullOrEmpty(input.Lastname);
        }
    }
}
