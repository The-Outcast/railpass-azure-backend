using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Railpass.Core;

namespace Railpass.Azure
{
    public class RailpassTimed
    {
        [FunctionName("UpdateAddressInfo")]
        public static async void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Timer function to retrieve address data started");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=nmbspocstorage;AccountKey=lC40NL3/aLq/c+XWw63A6kZPQGJPzoiOEtsKnjqFTNN7ftv1TNPIhKnBvvxhbJeIn/Jw8R//0hETRIVEzDKxsw==;EndpointSuffix=core.windows.net");
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("railpass");

            TableQuery<RailpassRequestEntity> query = new TableQuery<RailpassRequestEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "railpass"));

            // Execute the query and loop through the results
            var result = await table.ExecuteQuerySegmentedAsync(query, null);

            foreach (var railpassRequestEntity in result)
            {
                railpassRequestEntity.Address = $"Execution time of timer job: {DateTime.Now}";
                var updateOperation = TableOperation.Replace(railpassRequestEntity);

                await table.ExecuteAsync(updateOperation);
            }
        }
    }
}
