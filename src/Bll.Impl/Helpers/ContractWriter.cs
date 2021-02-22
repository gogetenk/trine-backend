using System;
using Assistance.Operational.Model;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Helpers
{
    public class ContractWriter
    {
        public UserModel Commercial { get; internal set; }
        public UserModel Consultant { get; internal set; }
        public UserModel Customer { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public string Title { get; internal set; }
        public float DailyPrice { get; internal set; }

        internal ContractWriter()
        {
        }

        public static ContractWriter Build()
        {
            return new ContractWriter();
        }

        public FrameContractModel WriteContract()
        {
            if (Consultant == null)
                throw new BusinessException("Consultant cannot be null to write a contract");

            if (Customer == null)
                throw new BusinessException("Customer cannot be null to write a contract");

            string content =
                "Contrat de prestation \n" +
                "Liant {0} à {1}" +
                ((Commercial != null) ? "Passant par l'intermédiaire {4}" : "") +
                "Allant de {2} à {3}" +
                "Lorem ipsum dolor sit amet...";

            content = string.Format(content, Consultant.Firstname + " " + Consultant.LastName, Customer.Firstname + " " + Customer.LastName, StartDate.ToString(), EndDate.ToString(), Commercial?.Firstname + " " + Commercial?.LastName);
            return new FrameContractModel()
            {
                Content = content,
                Status = ContractBaseModel.ContratStatus.CREATED,
                SubContracts = new System.Collections.Generic.List<SubContractModel>()
            };
        }
    }
}
