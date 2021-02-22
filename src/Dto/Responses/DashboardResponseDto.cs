using System;
using System.Collections.Generic;

namespace Dto.Responses
{
    public class DashboardResponseDto
    {
        public DashboardIndicatorsDto Indicators { get; set; }
        public List<ActivityDto> Activities { get; set; }
        public DashboardNewActivityBannerDto NewActivityBanner { get; set; }
    }

    public class DashboardActivityDto
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime SignatureDate { get; set; }
        public DateTime ValidationDate { get; set; }
        public float DaysNumber { get; set; }
        public ActivityStatusEnum Status { get; set; }
        public DashboardConsultantDto Consultant { get; set; }
        public DashboardConsultantDto Customer { get; set; }
    }

    public class DashboardConsultantDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Icon { get; set; }
    }

    public class DashboardIndicatorsDto
    {
        public DashboardYearIndicatorDto Year { get; set; }
        public DashboardMonthIndicatorDto Month { get; set; }
    }

    public class DashboardYearIndicatorDto
    {
        public DaysWorkedIndicatorDto TotalDaysWorked { get; set; }
        public DaysWorkedIndicatorDto AverageDaysWorked { get; set; }
    }

    public class DashboardMonthIndicatorDto
    {
        public DaysWorkedIndicatorDto TotalDaysWorked { get; set; }
    }

    public class DaysWorkedIndicatorDto
    {
        public float Value { get; set; }
        public float Trend { get; set; }
    }

    public class DashboardNewActivityBannerDto
    {
        public string ActivityId { get; set; }
    }
}
