namespace Dto
{
    public class ForgotPasswordResult
    {
        public ErrorCodes ErrorCode { get; set; }

        public enum ErrorCodes
        {
            EMAIL_UNKNOWN
        }
    }
}
