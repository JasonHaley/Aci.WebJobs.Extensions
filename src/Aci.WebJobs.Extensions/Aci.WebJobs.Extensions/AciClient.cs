using Aci.WebJobs.Extensions.Config;
using Aci.WebJobs.Extensions.Services;
using System.Threading.Tasks;

namespace Aci.WebJobs.Extensions
{
    public class AciClient
    {
        private readonly AciAttribute _attribute;
        private readonly AciConfiguration _configuration;
        private readonly IAciService _service;
        public AciClient(AciAttribute attribute, AciConfiguration configuration, IAciService service)
        {
            _attribute = attribute;
            _configuration = configuration;
            _service = service;
        }

        public async Task CreateAsync(string aciName, string containerImageName, int port)
        {
            await _service.CreateAsync(aciName, containerImageName, port);
        }
        public async Task CreateAsync(string containerImageName, int port)
        {
            await _service.CreateAsync(containerImageName, port);
        }

        public async Task DeleteAsync(string aciName)
        {
            await _service.DeleteAsync(aciName);
        }

        public async Task DeleteAsync()
        {
            await _service.DeleteAsync();
        }
        public async Task<string> GetLogContentAsync(string aciName)
        {
            return await _service.GetLogContentAsync(aciName);
        }
        public async Task<string> GetLogContentAsync()
        {
            return await _service.GetLogContentAsync();
        }

        public async Task<string> GetIpAddress(string aciName)
        {
            return await _service.GetIpAddress(aciName);
        }

        public async Task<string> GetIpAddress()
        {
            return await _service.GetIpAddress();
        }
    }
}
