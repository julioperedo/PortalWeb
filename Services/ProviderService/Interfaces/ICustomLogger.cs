using System.Threading.Tasks;

namespace ProviderService.Interfaces
{
    public interface ICustomLogger
    {
        void Log(string Message, string Subdirectory);
        Task ReportErrorByMail(string Message);
    }
}
