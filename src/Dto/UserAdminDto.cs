using System.Collections.Generic;

namespace Dto
{
    public class UserAdminDto
    {
        public UserDto User { get; set; }
        public TokenDto Token { get; set; }
        public List<MissionDto> Missions { get; set; }
        public List<ActivityDto> Activities { get; set; }
    }
}
