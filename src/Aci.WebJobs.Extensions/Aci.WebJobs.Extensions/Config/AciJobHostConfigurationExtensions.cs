
using Aci.WebJobs.Extensions.Config;
using Microsoft.Azure.WebJobs;
using System;

namespace Aci.WebJobs.Extensions
{
    public static class AciJobHostConfigurationExtensions
    {
        public static void UseAci(this JobHostConfiguration config, AciConfiguration aciConfig = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (aciConfig == null)
            {
                aciConfig = new AciConfiguration();
            }

            config.RegisterExtensionConfigProvider(aciConfig);
        }
        
    }
}
