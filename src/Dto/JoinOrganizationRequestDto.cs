namespace Dto
{
    public class JoinOrganizationRequestDto
    {
        public string Code { get; set; }
        public string UserId { get; set; }
        public RegisterUserRequestDto AccountData { get; set; }
    }
}
