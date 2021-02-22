using System;
using System.Collections.Generic;

namespace Assistance.Operational.Dto
{
    public class OrganizationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public List<OrganizationMemberDto> Members { get; set; }
        public string Icon { get; set; }
        public DateTime Created { get; set; }
    }
}
