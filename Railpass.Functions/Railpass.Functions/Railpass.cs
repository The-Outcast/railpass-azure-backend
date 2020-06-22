using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Railpass.Core;

namespace Railpass.Azure
{
    public static class Railpass
    {
        [FunctionName("Railpass")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nmbspocstorage;AccountKey=lC40NL3/aLq/c+XWw63A6kZPQGJPzoiOEtsKnjqFTNN7ftv1TNPIhKnBvvxhbJeIn/Jw8R//0hETRIVEzDKxsw==;EndpointSuffix=core.windows.net");
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("railpassqueue");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<RailpassRequest>(requestBody);

            await queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(new RailpassRequest
            {
                Insz = data.Insz,
                Email = data.Email,
                Firstname = data.Firstname,
                Lastname = data.Lastname
            })));

            return new OkResult();
        }

        [FunctionName("GetRailpassRequests")]
        public static async Task<IActionResult> GetRailpassRequests(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nmbspocstorage;AccountKey=lC40NL3/aLq/c+XWw63A6kZPQGJPzoiOEtsKnjqFTNN7ftv1TNPIhKnBvvxhbJeIn/Jw8R//0hETRIVEzDKxsw==;EndpointSuffix=core.windows.net");
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("railpass");

            TableQuery<RailpassRequestEntity> query = new TableQuery<RailpassRequestEntity>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "railpass"));

            // Execute the query and loop through the results
            var result = await table.ExecuteQuerySegmentedAsync(query, null);

            var railpassRequests = result.Results.Select(entity => new RailpassRequest
            {
                Firstname = entity.Firstname,
                Lastname = entity.Lastname,
                Email = entity.Email,
                Insz = entity.Insz,
                Address = entity.Address
            }).ToArray();

            return new OkObjectResult(JsonConvert.SerializeObject(railpassRequests));
        }
    }
}
