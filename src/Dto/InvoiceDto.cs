using System;

namespace Dto
{
    public class InvoiceDto
    {
        public string Id { get; set; }
        public string Number { get; set; } // Identifiant fonctionnel de la facture (ex : 2019-04) selon les lois en vigeurs (numéros qui se suivent etc)

        public string MissionId { get; set; }
        public string OrganizationId { get; set; } // Evite de récupérer la mission à chaque fois pour savoir de quelle orga vient la facture
        public string ProjectName { get; set; } // Nom du projet saisi depuis la mission

        public InvoiceUserDto Issuer { get; set; } // Infos sur la company emetrice
        public InvoiceUserDto Recipient { get; set; } // Infos sur la company receptrice

        public string Description { get; set; } // Objet à donner à la facture

        public float DailyRate { get; set; } // Le tjm
        public float WorkedDays { get; set; } // Nombere de jours du cra
        public int ActivityId { get; set; } // Id du CRA au cas où
        public float TaxRate { get; set; } // Pourcentage de TVA / taxes
        public float RawPrice { get; set; } // Prix HT
        public float TotalPrice { get; set; } // Prix TTC

        public DateTime CreatedDate { get; set; }
        public DateTime DueTo { get; set; }
        public StateEnum State { get; set; }

        public enum StateEnum
        {
            CREATED,
            WAITING_PAYMENT,
            PAID,
            CANCELED
        }
    }
}