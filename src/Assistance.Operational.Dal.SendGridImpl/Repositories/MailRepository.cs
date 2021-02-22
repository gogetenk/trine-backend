using System.Net;
using System.Threading.Tasks;
using Assistance.Operational.Dal.Repositories;
using Assistance.Operational.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.SendGridImpl.Repositories
{
    /// <summary>
    /// Repository responsible for sending mails notifications using send grid
    /// </summary>
    public class MailRepository : IMailRepository
    {
        private readonly ILogger<MailRepository> _logger;
        private readonly ISendGridClient _client;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public MailRepository(IConfiguration configuration, ILogger<MailRepository> logger, ISendGridClient client, IHostingEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _client = client;
            _env = env;
        }

        /// <summary>
        /// Sends an email to the specified user, using the specified event informations
        /// </summary>
        /// <param name="receiver">The user to send the email to.</param>
        /// <param name="generatedEvent">The event generated from the action. Contains all the data you need to create your mail.</param>
        /// <returns></returns>
        public async Task<bool> SendAsync(string mail, string templateId = "", object templateData = null, string subject = "", string content = "", MailAttachment attachment = null)
        {
            var message = new SendGridMessage();
            message.From = new EmailAddress(_configuration["Mail:DefaultAddress"], _configuration["Mail:DefaultName"]);

            if (!string.IsNullOrEmpty(templateId))
            {
                message.TemplateId = templateId;
            }

            if (!string.IsNullOrEmpty(content))
            {
                message.PlainTextContent = content;
            }

            var disableWhitelistValue = _configuration["Mail:DisableWhitelist"];
            var useWhitelist = !string.IsNullOrWhiteSpace(disableWhitelistValue) && !bool.Parse(disableWhitelistValue);
            var whitelist = _configuration["Mail:Whitelist"];
            if (useWhitelist && !whitelist.Contains(mail))
            {
                _logger.Log(LogLevel.Information, $"Email {mail} is not whitelisted");
                return false;
            }

            message.AddTo(mail);

            if (attachment != null)
            {
                await message.AddAttachmentAsync(attachment.Name, attachment.File, attachment.Type);
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                message.SetSubject(subject);
            }

            if (templateData != null)
            {
                message.SetTemplateData(templateData);
            }

            if (_env.EnvironmentName == "IntegrationTests")
                return true;

            Response response = await _client.SendEmailAsync(message);
            var body = await response.DeserializeResponseBodyAsync(response.Body);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                throw new DalException(JsonConvert.SerializeObject(body));

            return true;
        }
    }
}
