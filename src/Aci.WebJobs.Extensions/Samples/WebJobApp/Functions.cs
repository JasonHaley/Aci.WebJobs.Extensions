
using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace WebJobApp
{
    public static class Functions
    {
        public static async Task StartContainerInstance([QueueTrigger("create-aci")]string startQueue,
               [Aci(AciName = "juice-shop", Action = AciAction.Create)] AciClient aciClient,
               TraceWriter log)
        {
            log.Info("Creating container instance...");

            await aciClient.CreateAsync("bkimminich/juice-shop", 3000);

            log.Info("Container created.");

            var logs = await aciClient.GetLogContentAsync();
            log.Info($"Container logs: {logs}");

            var ipAddress = await aciClient.GetIpAddress();
            log.Info($"IPAddress: {ipAddress}");
        }

        [FunctionName("DeleteContainerInstance")]
        public static async Task DeleteContainerInstance([QueueTrigger("delete-aci")]string deleteQueue,
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
