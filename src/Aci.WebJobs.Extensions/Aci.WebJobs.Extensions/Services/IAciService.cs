using System.Threading.Tasks;

namespace Aci.WebJobs.Extensions.Services
{
    public interface IAciService
    {
        Task CreateAsync(string containerImaageName, int port);
        Task DeleteAsync();
        Task<string> GetLogContentAsync();
        Task<string> GetIpAddress();
    }
}