using System;
using System.Collections.Generic;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class Activity : MongoEntityBase
    {
        public string MissionId { get; set; }
        public UserActivity Commercial { get; set; }
        public UserActivity Customer { get; set; }
        public UserActivity Consultant { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ActivityStatusEnum Status { get; set; }
        public List<GridDay> Days { get; set; }
        public List<ModificationProposal> ModificationProposals { get; set; }
    }

    public enum ActivityStatusEnum
    {
        Generated,
        ConsultantSigned,
        ModificationsRequired,
        CustomerSigned,
    }
}
