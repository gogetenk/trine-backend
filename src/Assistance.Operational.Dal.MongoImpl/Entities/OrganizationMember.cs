using System;

namespace Assistance.Operational.Dal.MongoImpl.Entities
{
    public class OrganizationMember
    {
        public string UserId { get; set; }
        public string Firstname { get; set; } // On stocke le nom et prenom pour éviter d'avoir à requêter l'API users pour avoir ce genre d'info qui ne change jamais 
        public string Lastname { get; set; }
        public string Icon { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime JoinedAt { get; set; }

        public enum RoleEnum
        {
            GUEST, // Ne peut uniquement voir le dashboard de l'organisation. Rôle par défaut lorsque l'on invite quelqu'un
            MEMBER, // Peut voir les projets de l'organisation et les missions dans lesquelles il joue un rôle
            CLERK, // Peut voir toutes les missions de tout le monde, mais en readonly (pour les RH par exemple)
            MANAGER, // Peut voir toutes les missions et les gérer. Peut changer les rôles des utilisateurs (il ne peut pas les faire passer à un rôle supérieur au sien)
            SUPER_MANAGER, // Peut gérer l'organisation (photo, membres etc) et kicker des gens
            ADMIN // Full power
        }
    }
}