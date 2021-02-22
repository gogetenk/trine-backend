using System;

namespace Assistance.Operational.Model
{
    public class MissionInviteModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string IssuerId { get; set; }
        public MissionRoleEnum GivenRole { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        public enum MissionRoleEnum
        {
            COMMERCIAL,
            CUSTOMER,
            CONSULTANT
        }
    }
}
