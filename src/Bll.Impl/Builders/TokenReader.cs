using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Assistance.Operational.Bll.Impl.Builders
{
    public class TokenReader
    {
        public bool Verify(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            return jwtHandler.CanReadToken(token);
        }

        public List<Claim> GetClaims(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwt = jwtHandler.ReadJwtToken(token);
            return jwt.Claims.ToList();
        }
    }
}
