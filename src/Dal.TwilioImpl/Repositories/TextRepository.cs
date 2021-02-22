using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Model;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Dal.TwilioImpl.Repositories
{
    public class TextRepository : ITextRepository
    {

        private readonly string _defaultPhoneNumber;

        public TextRepository(IConfiguration configuration)
        {
            _defaultPhoneNumber = configuration["Twilio:DefaultPhoneNumber"];
            string accountSid = configuration["Twilio:SID"];
            string authToken = configuration["Twilio:Token"];
            TwilioClient.Init(accountSid, authToken);
        }

        public string Send(UserModel user, string content)
        {
            var message = MessageResource.Create(
                body: content,
                from: new PhoneNumber(_defaultPhoneNumber),
                to: new PhoneNumber(user.PhoneNumber)
            );
            return message.Sid;
        }
    }
}
