using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assistance.Operational.WebApi.IntegrationTests.Filters
{
    internal class FakeUserFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            context.HttpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, "123"),
                        new Claim(ClaimTypes.Name, "Test user"),
                        new Claim(ClaimTypes.Email, "test@test.test"),
                        new Claim(ClaimTypes.Role, "Admin")
                    }));


        }
    }
}
