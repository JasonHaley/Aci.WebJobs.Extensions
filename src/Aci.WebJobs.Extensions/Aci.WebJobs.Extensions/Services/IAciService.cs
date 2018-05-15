using System.Threading.Tasks;

namespace Aci.WebJobs.Extensions.Services
{
    public interface IAciService
    {
        Task CreateAsync(string aciName, string containerImageName, int port);
        Task CreateAsync(string containerImageName, int port);
        Task DeleteAsync(string aciName);
        Task DeleteAsync();
        Task<string> GetLogContentAsync(string aciName);
        Task<string> GetLogContentAsync();
        Task<string> GetIpAddress(string aciName);
        Task<string> GetIpAddress();
    }
}