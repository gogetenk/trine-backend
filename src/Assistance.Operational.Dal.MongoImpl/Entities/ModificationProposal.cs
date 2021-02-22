using System;
using System.Collections.Generic;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class ModificationProposal
    {
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public Status CurrentStatus { get; set; }
        public string UserId { get; set; }
        public List<GridDay> Modifications { get; set; }

        public enum Status
        {
            Pending,
            Accepted,
            Rejected
        }
    }
}
