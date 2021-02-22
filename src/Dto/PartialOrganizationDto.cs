using static Assistance.Operational.Dto.OrganizationMemberDto;

namespace Dto
{
    public class PartialOrganizationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsOwner { get; set; }
        public RoleEnum UserRole { get; set; }
        public int MembersNb { get; set; }
        public int MissionsNb { get; set; }
    }
}
