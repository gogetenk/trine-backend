namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Siret { get; set; }
        public string Rcs { get; set; }
        public CompanyType Type { get; set; }
        public string TvaNumber { get; set; }
        public Address Address { get; set; }
        public BankDetails BankDetails { get; set; }

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
