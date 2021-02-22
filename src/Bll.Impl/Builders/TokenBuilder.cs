using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Assistance.Operational.Bll.Impl.Builders
{
    /// <summary>
    /// Handles token crypting from a set of data
    /// </summary>
    public class TokenBuilder
    {
        public string Key { get; internal set; }
        public string Issuer { get; internal set; }
        public List<Claim> Claims { get; internal set; }
        public long Expiration { get; internal set; }

        public TokenBuilder()
        {
        }

        public static TokenBuilder Build()
        {
            return new TokenBuilder();
        }

        public string GetToken()
        {
            var encodedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var creds = new SigningCredentials(encodedKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: Issuer,
              claims: Claims,
              expires: DateTime.UtcNow.AddSeconds(Expiration),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
