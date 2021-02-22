using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Assistance.Operational.Bll.Services;
using Intercom.Clients;
using Intercom.Core;
using Intercom.Data;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Bll.Services;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Bll.Impl.Services
{
    public class LeadService : ServiceBase, ILeadService
    {
        private const string _personalAccessToken = "dG9rOjUyODg0NGE1XzQ2MzVfNDU3Ml9hZTkwXzAzYzllNDkwNWIxNjoxOjA=";
        private const string _message = "An error occured while creating the lead.";
        private const string _sourceType = "Landing Page";

        public LeadService(ILogger<ServiceBase> logger) : base(logger)
        {
        }

        public Task<bool> CreateLead(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new BusinessException("Email cannot be null");

            var address = new MailAddress(email);
            if (address is null)
                throw new BusinessException("The email address is badly formatted.");

            var auth = new Authentication(_personalAccessToken);
            var leadClient = new ContactsClient(new Authentication(_personalAccessToken));
            var userClient = new UsersClient(new Authentication(_personalAccessToken));

            // Trying to get an existing lead with the same email
            var existingLeads = leadClient.List(email: email);
            User user = null;

            // If the lead(s) exists, we convert it into user
            if (existingLeads != null && existingLeads.total_count > 0 && existingLeads.contacts.FirstOrDefault() != null)
                user = leadClient.Convert(existingLeads.contacts.FirstOrDefault(), new User() { email = email, utm_source = _sourceType, signed_up_at = ConvertToTimestamp(DateTime.UtcNow) });
            else // else we create a new user from scratch
                user = userClient.Create(new User() { email = email, utm_source = _sourceType, signed_up_at = ConvertToTimestamp(DateTime.UtcNow) });

            if (user is null)
                throw new BusinessException(_message);

            return Task.FromResult(true);
        }

        private long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }
}
