using System;
using System.Threading.Tasks;

namespace Assistance.Operational.Dal.Repositories
{
    public interface IAzureStorageRepository
    {
        Task<Uri> UploadAsync(byte[] file, string name, string contentType);
    }
}
