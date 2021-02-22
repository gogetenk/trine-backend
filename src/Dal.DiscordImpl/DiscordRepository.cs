using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Assistance.Event.Dal.Repositories;
using Microsoft.Extensions.Configuration;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Dal.DiscordImpl
{
    /// <summary>
    /// Repository to send messages to our discord channel
    /// </summary>
    public class DiscordRepository : IInternalNotificationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public DiscordRepository(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task SendAsync(string message)
        {
            var client = _clientFactory.CreateClient();
            var eventWebhookUrl = _configuration["DiscordEventWebhook"];
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(eventWebhookUrl));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent("{\"content\":\" " + message + " \"}", Encoding.UTF8, "application/json");
            var result = await client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
                throw new DalException(await result.Content.ReadAsStringAsync());
        }
    }
}
