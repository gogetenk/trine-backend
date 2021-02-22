using System;

namespace Assistance.Operational.Model
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public AddressModel Address { get; set; }
        public string SignatureFileUrl { get; set; }
        public string ProfilePicUrl { get; set; }
        public BankDetailsModel BankDetails { get; set; }
        public string LegalContributionFileUrl { get; set; }
        public string CompanySiret { get; set; }
        public CompanyModel Company { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDummy { get; set; }

        // Utilisé uniquement dans le cas d'une recup de membres. Spécifique à l'orga depuis laquelle on le demande.
        // Est une string car un enum n'est pas nullable
        public string Role { get; set; }

        // Utilisé pour les tests de Chris (ignorer cette prop)
        public GlobalRoleEnum GlobalRole { get; set; }

        public enum GlobalRoleEnum
        {
            Customer,
            Consultant,
            Admin
        }
    }
}
