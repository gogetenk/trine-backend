namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class BankDetails
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string BankName { get; set; }
        public Address BankAddress { get; set; }
    }
}
