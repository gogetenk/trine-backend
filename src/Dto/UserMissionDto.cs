namespace Dto
{
    public class UserMissionDto
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string ProfilePicUrl { get; set; }
        public bool IsDummy { get; set; }

        // Utilisé pour les tests de Chris (ignorer cette prop)
        public GlobalRoleEnum GlobalRole { get; set; }

        public enum GlobalRoleEnum
        {
            Customer,
            Consultant,
            Admin
        }
    }
}
