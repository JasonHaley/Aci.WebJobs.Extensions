
using Aci.WebJobs.Extensions;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using nClam;
using System.Linq;

namespace WebJobVirusScanner
{
    public static class Functions
    {
        
        public static async Task StartScanRequest(
            [BlobTrigger("uploads/{name}.{extension}")] CloudBlockBlob input,
            string name,
            string extension,
            [Aci] AciClient client,
            [Queue("requests")] IAsyncCollector<ScanRequest> requests,
            [Blob("toscan/{name}.{extension}", FileAccess.Write)] Stream output,
            ILogger logger)
        {
            logger.LogInformation($"Blob name: {name}.{extension}");

            // start server container and get aci name and ipaddress
            var scanRequest = await StartScanServer(client);
            
            // get the name of the blob
            scanRequest.InputPath = $"{name}.{extension}";
            
            // download the blob for copying to toscan folder
            await input.DownloadToStreamAsync(output);

            // remove original blob
            await input.DeleteIfExistsAsync();

            // queue scan request
            await requests.AddAsync(scanRequest);

        }

        public static async Task<ScanRequest> StartScanServer(AciClient client)
        {
            var uniqueAciId = ("clamavsvr-" + Guid.NewGuid().ToString("N")).ToLower();
            await client.CreateAsync(uniqueAciId, "mkodockx/docker-clamav", 3310);

            var ipAddress = await client.GetIpAddress(uniqueAciId);

            return new ScanRequest()
            {
                AciName = uniqueAciId,
                IPAddress = ipAddress
            };
        }

        public static async Task ScanFile(
            [QueueTrigger("requests")] ScanRequest request,
            [Blob("toscan/{InputPath}", FileAccess.Read)] CloudBlockBlob input,
            [Blob("scanned/{InputPath}", FileAccess.Write)] Stream safeOutput,
            [Blob("quaratine/{InputPath}", FileAccess.Write)] Stream badOutput,
            [Queue("results")] IAsyncCollector<ScanResult> results,
            [Queue("scanners")] IAsyncCollector<ScannerDeleteRequest> scanners)
        {
            var clam = new ClamClient(request.IPAddress, 3310);
            var pingResult = await clam.PingAsync();

            var file = new MemoryStream();
            await input.DownloadToStreamAsync(file);

            var clamResult = await clam.SendAndScanFileAsync(file);
            
            var result = new ScanResult()
            {
                FileName = Path.GetFileName(request.InputPath)
            };

            switch (clamResult.Result)
            {
                case ClamScanResults.Clean:
                    Console.WriteLine("The file is clean!");
                    break;
                case ClamScanResults.VirusDetected:
                    result.HasVirus = true;
                    result.Message = clamResult.InfectedFiles.First().VirusName;

                    Console.WriteLine("Virus Found!");
                    Console.WriteLine("Virus name: {0}", result.Message);
                    break;
                case ClamScanResults.Error:
                    result.Message = clamResult.RawResult;
                    Console.WriteLine("Woah an error occured! Error: {0}", clamResult.RawResult);
                    break;
            }

            // move the blob to the right container
            if (result.HasVirus)
            {
                await file.CopyToAsync(badOutput);
            }
            else
            {
                await file.CopyToAsync(safeOutput);
            }

            // delete the blob
            await input.DeleteIfExistsAsync();

            await results.AddAsync(result);

            var scannerDelete = new ScannerDeleteRequest()
            {
                AciName = request.AciName
            };
            await scanners.AddAsync(scannerDelete);
        }

        public static async Task DeleteScannerContainer(
            [QueueTrigger("scanners")] ScannerDeleteRequest request,
            [Aci] AciClient client)
        {
            await client.DeleteAsync(request.AciName);
        }
    }

    public class ScannerDeleteRequest
    {
        public string AciName { get; set; }
    }

    public class ScanResult 
    {
        public string FileName { get; set; }
        public bool HasVirus { get; set; }
        public string Message { get; set; }
    }
    public class ScanRequest
    {
        public string AciName { get; set; }
        public string IPAddress { get; set; }
        public string InputPath { get; set; }
    }
}
