# Azure Functions and WebJob binding for Azure Container Instance (ACI)
Utility binding to use with Azure Functions and WebJobs that interact with Azure Container Instances

**This is still in the early stages.**

## Features
* Create a new container
* Delete a container
* Get the logs from a container
* Get the IPAddress of the container 

## QuickStart

### Prerequisites
The current implementation uses a Service Principal for authentication with your Azure account. You will need the 

You can create a Service Principal with the following Azure CLI command:
```
az ad sp create-for-rbac --name <name> --password <password>
```

Here is an example of creating a service principal (replace the username and password with your own):

```
az ad sp create-for-rbac --name spname --password Password123!
```
The result will look like this:
```
{
  "appId": "cfd2f8df-e248-41b8-873b-4275d703349a",
  "displayName": "spname",
  "name": "http://spname",
  "password": "Password123!",
  "tenant": "df3bf7bb-bdae-4012-98a2-2742c5c77183"
}
```

#### Configuration
AppSettings
You will need to have the following values in App Settings (either in Azure or if running locally in a local.settings.json file):

* AzureWebJobsStorage - this is a connection string to a storage account
* AzureWebJobsDashboard - this is a connection string to a storage account
* ClientId - this is the **appId** returned when you created the Service Principal
* TenantId - this is the **tenant** returned once you created the Service Principal
* ClientSecret - this is the **password** for the Service Principal

If you deploy to Azure you will also need to add the following App Setting due to the usage of the beta SDK:

* FUNCTIONS_EXTENSION_VERSION=beta

#### Usage

The following example will create a new ACI container named "juice-shop" when any message is received in the "create-aci" queue in the storage account in the App Setting for AzureWebJobsStorage.
```
 public static async Task StartContainerInstance(
            [QueueTrigger("create-aci")] string startQueue,
            [Aci(AciName = "juice-shop", Action = AciAction.Create)] AciClient aciClient,
            TraceWriter log)
        {
            log.Info("Creating container instance...");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            // Create a new Container Instance of the juice-shop image from DockerHub
            // and expose port 3000
            await aciClient.CreateAsync("bkimminich/juice-shop", 3000);

            stopWatch.Start();
            log.Info($"Container created in {stopWatch.Elapsed}");

            // Get the logs content for the container
            var logs = await aciClient.GetLogContentAsync();
            log.Info($"Container logs: {logs}");

            // Get the IPAddress for the container
            var ipAddress = await aciClient.GetIpAddress();
            log.Info($"IPAddress: {ipAddress}");
        }
```

The following example will delete the ACI and the resource group it was created in when any message is received in the "delete-aci" queue in the storage account in the App Setting for AzureWebJobsStroage.
```
public static async Task DeleteContainerInstance(
            [QueueTrigger("delete-aci")] string deleteQueue,
            [Aci(AciName="juice-shop", Action=AciAction.Delete)] AciClient aciClient,
            TraceWriter log)
        {
            // Get the logs content for the container
            var logs = await aciClient.GetLogContentAsync();
            log.Info($"Container logs: {logs}");

            log.Info($"Deleting container...");

            // Delete the container instance and the resource group
            await aciClient.DeleteAsync();

            log.Info("Container deleted.");
        }
```