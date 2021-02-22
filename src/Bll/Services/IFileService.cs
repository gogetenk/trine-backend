namespace Assistance.Operational.Bll.Services
{
    public interface IFileService
    {
        string UploadFile(string fileName, byte[] file);
    }
}
