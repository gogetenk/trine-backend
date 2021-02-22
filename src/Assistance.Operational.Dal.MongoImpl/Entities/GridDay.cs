using System;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class GridDay
    {
        public DateTime Day { get; set; }
        public bool IsOpen { get; set; }
        public DayPartEnum WorkedPart { get; set; }
        public Absence Absence { get; set; }
    }

    public enum DayPartEnum
    {
        None,
        Morning,
        Afternoon,
        Full
    }
}