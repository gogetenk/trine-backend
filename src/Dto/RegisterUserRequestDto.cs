namespace Dto
{
    public class RegisterUserRequestDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public GlobalRoleEnum Role { get; set; }

        public enum GlobalRoleEnum
        {
            Customer,
            Consultant,
            Admin
        }
    }
}
