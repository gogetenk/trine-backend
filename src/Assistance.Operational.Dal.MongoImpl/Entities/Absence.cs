namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class Absence
    {
        public ReasonEnum Reason { get; set; }
        public string Comment { get; set; }
    }

    public enum ReasonEnum
    {
        None,
        Formation,
        Holiday,
        Absent,
        Sickness,
        Recover
    }
}