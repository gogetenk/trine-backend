using System;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class Mission : MongoEntityBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float DailyPrice { get; set; }
        public float CommercialFeePercentage { get; set; }
        public FrameContract FrameContract { get; set; }
        public StatusEnum Status { get; set; }
        public bool IsTripartite { get; set; }
        public bool IsFreelance { get; set; }
        public FrequencyEnum PaymentFrequency { get; set; }

        public UserMission Commercial { get; set; }
        public UserMission Consultant { get; set; }
        public UserMission Customer { get; set; }

        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationIcon { get; set; }
        public string ProjectName { get; set; }
        public string OwnerId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime CanceledDate { get; set; }

        public enum StatusEnum
        {
            CREATED,
            CONFIRMED,
            CANCELED
        }
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
