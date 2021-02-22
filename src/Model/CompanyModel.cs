namespace Assistance.Operational.Model
{
    public class CompanyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Siret { get; set; }
        public string Rcs { get; set; }
        public CompanyType Type { get; set; }
        public string TvaNumber { get; set; }
        public AddressModel Address { get; set; }
        public BankDetailsModel BankDetails { get; set; }

        public enum CompanyType
        {
            MICRO,
            EURL,
            SA,
            SAS,
            SASU,
            SARL,
            EI
        }
    }
}
