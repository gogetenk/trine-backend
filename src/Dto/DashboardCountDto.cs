namespace Dto
{
    /// <summary>
    /// Contains the informations to show on the home dashboard UI
    /// </summary>
    public class DashboardCountDto
    {
        public int MissionCount { get; set; }
        public int ClientCount { get; set; }
        public int EventCount { get; set; }
        public UserDto CurrentUser { get; set; }
    }
}
