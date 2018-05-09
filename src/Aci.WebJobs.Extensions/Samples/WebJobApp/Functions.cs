using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebJobApp
{
    public static class Functions
    {
        public static async Task StartContainerInstance(
            [QueueTrigger("create-aci")] string startQueue,
            [Aci(AciName = "juice-shop", Action = AciAction.Create)] AciClient aciClient,
            TraceWriter log)
        {
            log.Info("Creating container instance...");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await aciClient.CreateAsync("bkimminich/juice-shop", 3000);

            stopWatch.Start();
            log.Info($"Container created in {stopWatch.Elapsed}");

            var logs = await aciClient.GetLogContentAsync();
            log.Info($"Container logs: {logs}");

            var ipAddress = await aciClient.GetIpAddress();
            log.Info($"IPAddress: {ipAddress}");
        }

        public static async Task DeleteContainerInstance(
            [QueueTrigger("delete-aci")]string deleteQueue,
            [Aci(AciName = "juice-shop", Action = AciAction.Delete)] AciClient aciClient,
            TraceWriter log)
        {
            var logs = await aciClient.GetLogContentAsync();
            log.Info($"Container logs: {logs}");

            log.Info($"Deleting container...");

            await aciClient.DeleteAsync();

            log.Info("Container deleted.");
        }
        
    }
}
