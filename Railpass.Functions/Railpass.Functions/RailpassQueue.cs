using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Railpass.Core;

namespace Raillpass.Azure
{
    public static class RailpassQueue
    {
        [FunctionName("RailpassQueue")]
        public static void Run([QueueTrigger("railpassqueue")]RailpassRequest myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nmbspocstorage;AccountKey=lC40NL3/aLq/c+XWw63A6kZPQGJPzoiOEtsKnjqFTNN7ftv1TNPIhKnBvvxhbJeIn/Jw8R//0hETRIVEzDKxsw==;EndpointSuffix=core.windows.net");
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("railpass");

            var operation = TableOperation.InsertOrReplace(new RailpassEntity(myQueueItem.Insz)
            {
                Lastname = myQueueItem.Lastname,
                Firstname = myQueueItem.Firstname,
                Email = myQueueItem.Email,
                Timestamp = DateTimeOffset.Now
            });

            table.ExecuteAsync(operation);
        }
    }

    public class RailpassEntity : TableEntity
    {
        private const string RailpassPartitionKey = "railpass";

        public RailpassEntity(string insz) : base(RailpassPartitionKey, insz)
        {
            
        }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}
