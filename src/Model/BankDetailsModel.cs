namespace Assistance.Operational.Model
{
    public class BankDetailsModel
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string BankName { get; set; }
        public AddressModel BankAddress { get; set; }
    }
}
