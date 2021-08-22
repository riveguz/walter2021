using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using Walter2021.Function.Entities;

namespace Walter2021.Function.Funtions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
            [Table("walter", Connection = "AzureWebJobsStorage")] CloudTable walterTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");


            string filter = TableQuery.GenerateFilterConditionForBool("IsCompleted", QueryComparisons.Equal, true);
            TableQuery<WalterEntity> query = new TableQuery<WalterEntity>().Where(filter);
            TableQuerySegment<WalterEntity> completedWalters = await walterTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach(WalterEntity completedWalter in completedWalters)
            {
                await walterTable.ExecuteAsync(TableOperation.Delete(completedWalter));
                deleted++;
            }

            log.LogInformation($"Deleted: {deleted} items at: {DateTime.Now}");
        }
    }
}

