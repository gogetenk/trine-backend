using Assistance.Operational.Model;

namespace Assistance.Operational.Dal.Repositories
{
    public interface ITextRepository
    {
        string Send(UserModel receiver, string content);
    }
}
