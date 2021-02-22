namespace Dto.Requests
{
    public class CreateUserRequestDto
    {
        public ExternalUserDto User { get; set; }
        public string SecretToken { get; set; }
    }
}
