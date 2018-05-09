using System;
using Aci.WebJobs.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionApp
{
    public static class AciFunctions
    {
        [FunctionName("CreateContainerInstance")]
        public static void StartContainerInstance([QueueTrigger("create-aci")]string startQueue, 
            [Aci(ContainerImageName="", Action=AciAction.Create)] AciClient aciClient,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {startQueue}");
        }

        [FunctionName("DeleteContainerInstance")]
        public static void DeleteContainerInstance([QueueTrigger("delete-aci")]string deleteQueue,
            [Aci(ContainerImageName="", Action=AciAction.Delete)] AciClient aciClient,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {deleteQueue}");
        }

        [FunctionName("GetLogsContainerInstance")]
        public static void GetLogsContainerInstance([TimerTrigger("00:00:30")] TimerInfo timer,
            [Aci(Action=AciAction.Logs)] AciClient client,
            TraceWriter log)
        {

        }
    }
}
