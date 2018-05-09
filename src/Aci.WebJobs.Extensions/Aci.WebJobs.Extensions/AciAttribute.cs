using Microsoft.Azure.WebJobs.Description;
using System;

namespace Aci.WebJobs.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class AciAttribute : Attribute
    {
        [AutoResolve]
        public string ContainerImageName { get; set; }
        [AutoResolve]
        public string AciName { get; set; }
        [AutoResolve]
        public string Region { get; set; } = "eastus";
        [AutoResolve]
        public string ResourceGroupName { get; set; } = "aci-rg";
        
        public AciAction Action { get; set; }

        public int Port { get; set; }

        [AppSetting]
        public string ClientId { get; set; }
        [AppSetting]
        public string ClientSecret { get; set; }
        [AppSetting]
        public string TenantId { get; set; }
    }

    public enum AciAction
    {
        Create = 0,
        Delete = 1,
        Logs = 2
    }
}
