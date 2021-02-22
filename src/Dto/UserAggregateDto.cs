using System;
using Newtonsoft.Json;

namespace Dto
{
    public class UserAggregateDto
    {
        public string Id { get; set; }
        [JsonProperty(PropertyName = "firstName")]
        public string Firstname { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public AddressDto Address { get; set; }
        public string SignatureFileUrl { get; set; }
        public string ProfilePicUrl { get; set; }
        public BankDetailsDto BankDetails { get; set; }
        public string LegalContributionFileUrl { get; set; }
        public CompanyDto Company { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDummy { get; set; }

        public enum UserTypeEnum
        {
            COMMERCIAL,
            CUSTOMER,
            CONSULTANT
        }
    }
}
