using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assistance.Operational.Dal.Repositories;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.AzureNotificationImpl.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ILogger _logger;
        private readonly NotificationHubClient _hub;

        private const string _connectionString = "Endpoint=sb://hub-assistance.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=u3YN78r4CrUOiaSZgh9tLo3q28A++dA3TZNdYA9EzUU=";
        private const string _notificationHubName = "hub-assistance";

        public NotificationRepository(ILogger<NotificationRepository> logger)
        {
            _logger = logger;
            _hub = NotificationHubClient.CreateClientFromConnectionString(_connectionString, _notificationHubName, false); //TODO: A virer en prod (le true)
        }

        /// <summary>
        /// Sends a notification to Azure Notification hub
        /// </summary>
        /// <param name="content">The content of the notification</param>
        /// <param name="userIds">A nullable list of userids. Send null if you want to broadcast.</param>
        /// <returns></returns>
        public async Task SendTemplatedNotification(Dictionary<string, string> content, List<string> userIds)
        {
            try
            {
                if (content is null || !content.Any())
                    throw new TechnicalException("Parameters null");

                // TODO: Vérifier si c'est vraiment 20 tags max (donc 20 users)
                var result = await _hub.SendTemplateNotificationAsync(content, userIds);
                if (result.State > NotificationOutcomeState.Completed)
                    throw new DalException($"The azure notification hub returned with an error : {result.State}");

                _logger.LogTrace("[Push notif] Notification sent without error.");
            }
            catch (Exception exc)
            {
                _logger.LogError("An error occured while sending a push notification to Azure.", exc);
                throw;
            }
        }
    }
}
