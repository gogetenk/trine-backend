using System;
using Assistance.Operational.Bll.Impl.Builders;
using Dto;
using Microsoft.Extensions.Configuration;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public static class TokenHelper
    {
        private const string AuthenticationConfigSectionName = "AuthenticationSettings";

        public static TokenDto BuildToken(IConfiguration configuration, UserDto user)
        {
            try
            {
                long tokenExpirationInSeconds;
                long.TryParse(configuration[AuthenticationConfigSectionName + ":TokenExpirationInSeconds"], out tokenExpirationInSeconds);

                var accessToken = TokenBuilder.Build()
                    .WithKey(configuration[AuthenticationConfigSectionName + ":Key"])
                    .WithIssuer(configuration[AuthenticationConfigSectionName + ":Issuer"])
                    .WithExpiration(tokenExpirationInSeconds)
                    .WithUserBasedClaims(user)
                    .GetToken();


                return new TokenDto()
                {
                    UserId = user.Id,
                    UserFirstname = user.Firstname,
                    UserLastname = user.Lastname,
                    UserSubscriptionDate = user.SubscriptionDate,
                    UserMail = user.Email,
                    AccessToken = accessToken,
                    ExpiresIn = tokenExpirationInSeconds,
                    TokenType = configuration[AuthenticationConfigSectionName + ":TokenType"],
                };
            }
            catch (BusinessException bExc)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new TechnicalException("An error occured while creating token", exc);
            }
        }
    }
}
