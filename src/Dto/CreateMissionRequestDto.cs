using System;

namespace Dto
{
    public class CreateMissionRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float DailyPrice { get; set; }
        public float CommercialFeePercentage { get; set; }
        public bool IsTripartite { get; set; }
        public bool IsFreelance { get; set; }
        public FrequencyEnum PaymentFrequency { get; set; }

        // FKs
        public UserDto Commercial { get; set; }
        public UserDto Consultant { get; set; }
        public UserDto Customer { get; set; }
        public string OrganizationId { get; set; }
        public string ProjectName { get; set; }
        public string OwnerId { get; set; }

        public enum FrequencyEnum
        {
            DAILY,
            WEEKLY,
            MONTHLY,
            ANNUALLY,
            ONTERM
        }
    }
}
