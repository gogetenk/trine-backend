using Dto;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public class ActivityHelper
    {

        public static double GetNumericWorkedPart(DayPartEnum workedPart)
        {
            switch (workedPart)
            {
                case DayPartEnum.Full:
                    return 1;
                case DayPartEnum.Morning:
                case DayPartEnum.Afternoon:
                    return 0.5;
                default:
                    return 0;
            }
        }

    }
}
