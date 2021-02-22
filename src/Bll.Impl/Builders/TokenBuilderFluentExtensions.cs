using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Dto;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Builders
{
    /// <summary>
    /// Extensions method for the TokenBuilder (in a Fluent way)
    /// </summary>
    public static class TokenBuilderFluentExtensions
    {
        /// <summary>
        /// Adds a signing key to the token encryption process
        /// </summary>
        /// <param name="builder">The actual TokenBuilder instance</param>
        /// <param name="key">The singing Key (supposed to be base 64 encoded) </param>
        /// <returns></returns>
        public static TokenBuilder WithKey(this TokenBuilder builder, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new BusinessException("The jwt signing key has to be specified in the settings.");

            builder.Key = key;
            return builder;
        }

        /// <summary>
        /// Adds the issuer to the token encryption process
        /// </summary>
        /// <param name="builder">The actual TokenBuilder instance</param>
        /// <param name="issuer">A string representing the authentication authority (usually an URL)</param>
        /// <returns></returns>
        public static TokenBuilder WithIssuer(this TokenBuilder builder, string issuer)
        {
            if (string.IsNullOrEmpty(issuer))
                throw new BusinessException("The jwt issuer has to be specified in the settings.");

            builder.Issuer = issuer;
            return builder;
        }

        /// <summary>
        /// Specifies the expiration of the token in seconds
        /// </summary>
        /// <param name="builder">The actual TokenBuilder instance</param>
        /// <param name="expiration">The lifespan of the token in seconds</param>
        /// <returns></returns>
        public static TokenBuilder WithExpiration(this TokenBuilder builder, long expiration)
        {
            if (expiration <= 0)
                throw new BusinessException("The expiration has to be greater than zero seconds.");

            builder.Expiration = expiration;
            return builder;
        }

        /// <summary>
        /// Adds claims representing the user to the token
        /// </summary>
        /// <param name="builder">The actual TokenBuilder instance</param>
        /// <param name="user">The user that holds the needed claims</param>
        /// <returns></returns>

        public static TokenBuilder WithUserBasedClaims(this TokenBuilder builder, UserDto user)
        {
            if (user == null)
                throw new BusinessException("The user cannot be null while creating associated claims.");

            var claims = new List<Claim>()
            {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email)
            };
            builder.Claims = claims;

            return builder;
        }

        /// <summary>
        /// Adds an email in the claims
        /// </summary>
        public static TokenBuilder WithEmailClaim(this TokenBuilder builder, string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new BusinessException("The email cannot be null while creating associated claim.");

            if (builder.Claims is null || !builder.Claims.Any())
                builder.Claims = new List<Claim>();

            builder.Claims.Add(new Claim("customerEmail", email));
            return builder;
        }

        /// <summary>
        /// Adds an activity id in the claims
        /// </summary>
        public static TokenBuilder WithActivityIdClaim(this TokenBuilder builder, string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                throw new BusinessException("The activity identifier cannot be null while creating associated claim.");

            if (builder.Claims is null || !builder.Claims.Any())
                builder.Claims = new List<Claim>();

            builder.Claims.Add(new Claim("activityId", activityId));
            return builder;
        }
    }
}
