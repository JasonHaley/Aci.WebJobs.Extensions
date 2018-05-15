using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace WebJobVirusScanner
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
