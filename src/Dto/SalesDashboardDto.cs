using System.Collections.Generic;

namespace Dto
{
    public class SalesDashboardDto
    {
        public double WorkedDays { get; set; }
        public int TotalDays { get; set; }
        public double Ratio { get; set; }
        public double SalesRevenue { get; set; }

        public List<MissionDto> Missions { get; set; }
    }
}
