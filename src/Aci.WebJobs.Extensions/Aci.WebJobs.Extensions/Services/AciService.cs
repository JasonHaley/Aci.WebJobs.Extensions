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

        public async Task CreateAsync(string containerImageName, int port)
        {
            await CreateAsync(_attribute.AciName, containerImageName, port);
        }

        public async Task CreateAsync(string aciName, string containerImageName, int port)
        {
            await _azure.ContainerGroups.Define($"{aciName}-group")
                        .WithRegion(_attribute.Region)
                        .WithNewResourceGroup(_attribute.ResourceGroupName)
                        .WithLinux()
                        .WithPublicImageRegistryOnly()
                        .WithoutVolume()
                        .DefineContainerInstance(aciName)
                        .WithImage(containerImageName)
                        .WithExternalTcpPort(port)
                        .WithCpuCoreCount(.5)
                        .WithMemorySizeInGB(1)
                        .Attach()
                        .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                        .CreateAsync();
        }

        public async Task DeleteAsync()
        {
            await DeleteAsync(_attribute.AciName);
        }
        public async Task DeleteAsync(string aciName)
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                _attribute.ResourceGroupName,
                $"{aciName}-group");

            _azure.ContainerGroups.DeleteById(containerGroup.Id);

            await _azure.ResourceGroups.BeginDeleteByNameAsync(_attribute.ResourceGroupName);
        }

        public async Task<string> GetLogContentAsync()
        {
            return await GetLogContentAsync(_attribute.AciName);
        }

        public async Task<string> GetLogContentAsync(string aciName)
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                    _attribute.ResourceGroupName,
                    $"{aciName}-group");

            return containerGroup.GetLogContent(_attribute.AciName);
        }

        public async Task<string> GetIpAddress()
        {
            return await GetIpAddress(_attribute.AciName);
        }
        public async Task<string> GetIpAddress(string aciName)
        {
            var containerGroup = await _azure.ContainerGroups.GetByResourceGroupAsync(
                    _attribute.ResourceGroupName,
                    $"{aciName}-group");
            return containerGroup.IPAddress;
        }
    }
}
