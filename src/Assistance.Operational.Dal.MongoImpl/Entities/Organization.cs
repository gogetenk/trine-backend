using System;
using System.Collections.Generic;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class Organization : MongoEntityBase
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public List<OrganizationMember> Members { get; set; }
        public string Icon { get; set; }
        public DateTime Created { get; set; }
    }
}
