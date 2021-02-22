namespace Assistance.Operational.WebApi.Configurations
{
    /// <summary>
    /// Authentication options 
    /// </summary>
    public class AuthenticationSettings
    {
        /// <summary>
        /// Authentication server encryption key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Accepted issuer for the provided token
        /// </summary>
        public string Issuer { get; set; }
        public string Authority { get; set; }
        public string Audience { get; set; }
    }
}
