using System;
using System.Collections.Generic;

namespace Dto
{
    public class MissionDto
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float DailyPrice { get; set; }
        public float CommercialFeePercentage { get; set; }
        public FrameContractDto FrameContract { get; set; }
        public StatusEnum Status { get; set; }
        public bool IsTripartite { get; set; }
        public bool IsFreelance { get; set; }
        public FrequencyEnum PaymentFrequency { get; set; }

        public UserMissionDto Commercial { get; set; }
        public UserMissionDto Consultant { get; set; }
        public UserMissionDto Customer { get; set; }

        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationIcon { get; set; }
        public string ProjectName { get; set; }
        public string OwnerId { get; set; }

        public List<InvoiceDto> Invoices { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public List<EventDto> Events { get; set; }

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
