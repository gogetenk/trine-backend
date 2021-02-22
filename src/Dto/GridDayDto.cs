using System;

namespace Dto
{
    public class GridDayDto
    {
        public DateTime Day { get; set; }
        public bool IsOpen { get; set; }
        public DayPartEnum WorkedPart { get; set; }
        public AbsenceDto Absence { get; set; }
    }

    public enum DayPartEnum
    {
        None,
        Morning,
        Afternoon,
        Full
    }
}
