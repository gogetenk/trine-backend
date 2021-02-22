using System;

namespace Dto
{
    public class CountryDto
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }
        public DateTimeOffset LocalUtcOffset { get; set; }
    }
}
