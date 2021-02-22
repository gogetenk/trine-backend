using System;
using Assistance.Operational.Model;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public static class ContractWriterFluentExtensions
    {
        public static ContractWriter WithCommercial(this ContractWriter writer, UserModel user)
        {
            if (user == null)
                return writer;

            writer.Commercial = user;
            return writer;
        }

        public static ContractWriter WithCustomer(this ContractWriter writer, UserModel user)
        {
            writer.Customer = user;
            return writer;
        }

        public static ContractWriter WithConsultant(this ContractWriter writer, UserModel user)
        {
            writer.Consultant = user;
            return writer;
        }

        public static ContractWriter WithDailyPrice(this ContractWriter writer, float dailyPrice)
        {
            writer.DailyPrice = dailyPrice;
            return writer;
        }

        public static ContractWriter From(this ContractWriter writer, DateTime date)
        {
            writer.StartDate = date;
            return writer;
        }

        public static ContractWriter To(this ContractWriter writer, DateTime date)
        {
            writer.EndDate = date;
            return writer;
        }

        public static ContractWriter WithTitle(this ContractWriter writer, string title)
        {
            writer.Title = title;
            return writer;
        }
    }
}
