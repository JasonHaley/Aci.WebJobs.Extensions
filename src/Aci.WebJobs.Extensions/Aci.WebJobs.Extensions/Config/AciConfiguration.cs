using Aci.WebJobs.Extensions.Services;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Aci.WebJobs.Extensions.Config
{
    public class AciConfiguration : IExtensionConfigProvider
    {

        public void Initialize(ExtensionConfigContext context)
        {
            var binding = context.AddBindingRule<AciAttribute>()
                                 .BindToInput<AciClient>(attr => new AciClient(attr, this, CreateService(attr)));

        }

        internal IAciService CreateService(AciAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.ClientId))
            {
                attribute.ClientId = AmbientConnectionStringProvider.Instance.GetConnectionString("ClientId");
            }

            if (string.IsNullOrEmpty(attribute.ClientSecret))
            {
                attribute.ClientSecret = AmbientConnectionStringProvider.Instance.GetConnectionString("ClientSecret");
            }

            if (string.IsNullOrEmpty(attribute.TenantId))
            {
                attribute.TenantId = AmbientConnectionStringProvider.Instance.GetConnectionString("TenantId");
            }

            return new AciService(attribute);
        }
    }
}
