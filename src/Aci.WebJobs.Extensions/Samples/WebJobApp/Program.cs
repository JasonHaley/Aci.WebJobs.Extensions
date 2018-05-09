using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebJobApp
{
    class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            config.UseAci();

            config.NameResolver = new NameResolver();

            config.LoggerFactory.AddConsole();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            JobHost host = new JobHost(config);
            host.RunAndBlock();
            
        }
    }
}
