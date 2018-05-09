using Aci.WebJobs.Extensions.Config;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Threading.Tasks;

namespace Aci.WebJobs.Extensions.Services
{
    internal class AciService : IAciService
    {
        AzureCredentials _creds;
        AciAttribute _attribute;
        IAzure _azure;
        
        public AciService(AciAttribute attribute)
        {
            _attribute = attribute;
            Initialize();
        }

        private void Initialize()
        {
            _creds = new AzureCredentialsFactory().FromServicePrincipal(
                _attribute.ClientId, 
                _attribute.ClientSecret, 
                _attribute.TenantId, 
                AzureEnvironment.AzureGlobalCloud);

            _azure = Azure.Configure()
                          .Authenticate(_creds)
                          .WithDefaultSubscription();
        }

        public async Task CreateAsync(string containerImaageName, int port)
        {
            await _azure.ContainerGroups.Define($"{_attribute.AciName}-group")
                        .WithRegion(_attribute.Region)
                        .WithNewResourceGroup(_attribute.ResourceGroupName)
                        .WithLinux()
                        .WithPublicImageRegistryOnly()
                        .WithoutVolume()
                        .DefineContainerInstance(_attribute.AciName)
                        .WithImage(containerImaageName)
                        .WithExternalTcpPort(port)
                        .WithCpuCoreCount(.5)
                        .WithMemorySizeInGB(1)
                        .Attach()
                        .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                        .CreateAsync();
        }
        
        public async Task DeleteAsync()
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                _attribute.ResourceGroupName,
                $"{_attribute.AciName}-group");

            _azure.ContainerGroups.DeleteById(containerGroup.Id);

            await _azure.ResourceGroups.BeginDeleteByNameAsync(_attribute.ResourceGroupName);
        }

        public async Task<string> GetLogContentAsync()
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                    _attribute.ResourceGroupName,
                    $"{_attribute.AciName}-group");

            return containerGroup.GetLogContent(_attribute.AciName);
        }

        public async Task<string> GetIpAddress()
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                    _attribute.ResourceGroupName,
                    $"{_attribute.AciName}-group");
            return containerGroup.IPAddress;
        }
    }
}
