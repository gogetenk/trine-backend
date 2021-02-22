using System;
using System.Collections.Generic;

namespace Dto
{
    public class ActivityDto
    {
        public string Id { get; set; }
        public string MissionId { get; set; }
        public float DaysNumber { get; set; }
        public UserActivityDto Commercial { get; set; }
        public UserActivityDto Customer { get; set; }
        public UserActivityDto Consultant { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ActivityStatusEnum Status { get; set; }
        public List<GridDayDto> Days { get; set; }
        public List<ModificationProposalDto> ModificationProposals { get; set; }
    }

    public enum ActivityStatusEnum
    {
        Generated,
        ConsultantSigned,
        ModificationsRequired,
        CustomerSigned,
    }
}
