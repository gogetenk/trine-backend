using System.Threading.Tasks;
using Dto;

namespace Assistance.Operational.Bll.Services
{
    public interface IAccountService
    {
        Task<CreatedUserDto> RegisterUser(RegisterUserRequestDto dto);
        Task<string> RegisterCompany(RegisterCompanyRequestDto dto);
        Task<TokenDto> AuthenticateUser(UserCredentialsDto login);
        Task<CompanyDto> CheckSiret(string siret);
        Task<UserDto> CheckMail(string email);
        Task<UserDto> GetUserById(string id);
        Task<bool> SendPasswordRecoveryEmail(ForgotPasswordDto dto);
        Task<string> UpdateUserPassword(string token);
        Task SendEmailInvitations(SubscriptionInvitationDto request, string userId);
    }
}
