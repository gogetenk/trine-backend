using System;
using System.Collections.Generic;

namespace Dto
{
    public class ModificationProposalDto
    {
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public Status CurrentStatus { get; set; }
        public string UserId { get; set; }
        public List<GridDayDto> Modifications { get; set; }

        public enum Status
        {
            Pending,
            Accepted,
            Rejected
        }
    }
}
