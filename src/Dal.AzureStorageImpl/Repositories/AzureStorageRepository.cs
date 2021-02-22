using System;
using System.Threading.Tasks;
using Assistance.Operational.Dal.Repositories;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.AzureStorageImpl.Repositories
{
    public class AzureStorageRepository : IAzureStorageRepository
    {

        private readonly CloudBlobContainer _container;
        private readonly string CONTAINER_NAME = "uploads";
        private readonly ILogger<AzureStorageRepository> _logger;

        public AzureStorageRepository(IConfiguration configuration, ILogger<AzureStorageRepository> logger)
        {
            _logger = logger;
            var azureStorageConnectionString = configuration["AZURE_STORAGE_CONNECTION"];
            if (string.IsNullOrEmpty(azureStorageConnectionString))
            {
                throw new DalException("Azure connection string is missing");
            }

            CloudStorageAccount.TryParse(azureStorageConnectionString, out CloudStorageAccount account);
            if (account is null)
            {
                _logger.LogWarning("Warning : the azure storage connection string seems wrong.");
                return;
            }
            var client = account.CreateCloudBlobClient();
            _container = client.GetContainerReference(CONTAINER_NAME);
            _container.CreateIfNotExists();
        }

        public async Task<Uri> UploadAsync(byte[] file, string name, string contentType)
        {
            try
            {
                var blob = _container.GetBlockBlobReference(name);
                blob.Properties.ContentType = contentType;
                await blob.UploadFromByteArrayAsync(file, 0, file.Length);
                return blob.Uri;
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                return null;
            }
        }
    }
}
