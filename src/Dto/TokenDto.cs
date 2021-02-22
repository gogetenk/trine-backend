using System;

namespace Dto
{
    /// <summary>
    /// An OAuth 2.0 Access Token that is formatted as a JWT.
    /// </summary>
    public class TokenDto
    {
        public string UserId { get; set; }
        public string UserFirstname { get; set; }
        public string UserLastname { get; set; }
        public string UserMail { get; set; }
        public DateTime UserSubscriptionDate { get; set; }
        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
