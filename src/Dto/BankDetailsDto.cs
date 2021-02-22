namespace Dto
{
    public class BankDetailsDto
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string BankName { get; set; }
        public AddressDto BankAddress { get; set; }
    }
}