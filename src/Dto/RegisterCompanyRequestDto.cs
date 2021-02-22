using static Dto.CompanyDto;

namespace Dto
{
    public class RegisterCompanyRequestDto
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Siret { get; set; }
        public string Rcs { get; set; }
        public string TvaNumber { get; set; }
        public AddressDto Address { get; set; }
        public CompanyType Type { get; set; }
    }
}
