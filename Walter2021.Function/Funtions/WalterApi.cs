using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Walter2021.Common.Models;
using Walter2021.Common.Responses;
using Walter2021.Function.Entities;

namespace Walter2021.Function.Funtions
{
    public static class WalterApi
    {
        [FunctionName(nameof(CreateWalter))]
        public static async Task<IActionResult> CreateWalter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "walter")] HttpRequest req,
            [Table("walter", Connection = "AzureWebJobsStorage")] CloudTable walterTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new walter");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Walter walter = JsonConvert.DeserializeObject<Walter>(requestBody);

            if (string.IsNullOrEmpty(walter?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaksDescription."
                });
            }

            WalterEntity walterEntity = new WalterEntity
            {
                CreateTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "WALTER",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = walter.TaskDescription
            };

            TableOperation addOperation = TableOperation.Insert(walterEntity);
            await walterTable.ExecuteAsync(addOperation);

            string message = "New walter stored in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = walterEntity
            });
        }

        [FunctionName(nameof(UpdateWalter))]
        public static async Task<IActionResult> UpdateWalter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "walter/{id}")] HttpRequest req,
            [Table("walter", Connection = "AzureWebJobsStorage")] CloudTable walterTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for walter: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Walter walter = JsonConvert.DeserializeObject<Walter>(requestBody);

            //Validate walter id
            TableOperation findOperation = TableOperation.Retrieve<WalterEntity>("WALTER", id);
            TableResult findResult = await walterTable.ExecuteAsync(findOperation);
            if(findResult.Result ==null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Walter not found"
                });
            }

            //Update walter
            WalterEntity walterEntity = (WalterEntity)findResult.Result; 
            walterEntity.IsCompleted = walter.IsCompleted;
            if(!string.IsNullOrEmpty(walter.TaskDescription))
            {
                walterEntity.TaskDescription = walter.TaskDescription;
            }

            TableOperation addOperation = TableOperation.Replace(walterEntity);
            await walterTable.ExecuteAsync(addOperation);

            string message = $"Walter: {id}, update in table.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = walterEntity
            });
        }
    }
}
