
using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace WebJobApp
{
    public static class Functions
    {
        public static async Task StartContainerInstance([QueueTrigger("create-aci")]string startQueue,
               [Aci(ContainerImageName = "bkimminich/juice-shop", Port = 3000, AciName = "juice-shop", Action = AciAction.Create)] AciClient aciClient,
               TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {startQueue}");

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
            log.Info($"C# Queue trigger function processed: {deleteQueue}");

            await aciClient.DeleteAsync();
        }

        //[FunctionName("GetLogsContainerInstance")]
        //public static void GetLogsContainerInstance([TimerTrigger("00:00:30")] TimerInfo timer,
        //    [Aci(Action = AciAction.Logs)] AciClient client,
        //    TraceWriter log)
        //{

        //}
    }
}
