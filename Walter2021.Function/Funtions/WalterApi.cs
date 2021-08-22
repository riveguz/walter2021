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
            if (findResult.Result == null)
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
            if (!string.IsNullOrEmpty(walter.TaskDescription))
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

        [FunctionName(nameof(GetAllWalters))]
        public static async Task<IActionResult> GetAllWalters(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "walter")] HttpRequest req,
            [Table("walter", Connection = "AzureWebJobsStorage")] CloudTable walterTable,
           ILogger log)
        {
            log.LogInformation("Get all walter received");

            TableQuery<WalterEntity> query = new TableQuery<WalterEntity>();
            TableQuerySegment<WalterEntity> walter = await walterTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieve all walter";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = walter
            });
        }

        [FunctionName(nameof(GetWalterById))]
        public static IActionResult GetWalterById(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "walter/{id}")] HttpRequest req,
          [Table("walter", "WALTER", "{id}", Connection = "AzureWebJobsStorage")] WalterEntity walterEntity,
          string id,
          ILogger log)
        {
            log.LogInformation($"Get walter by id: {id}, received");

            if (walterEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Walter not found"
                });
            }

            string message = $"Walter: {walterEntity.RowKey}, Retrieve";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = walterEntity
            });
        }

        [FunctionName(nameof(DeleteWalter))]
        public static async Task<IActionResult> DeleteWalter(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "walter/{id}")] HttpRequest req,
           [Table("walter", "WALTER", "{id}", Connection = "AzureWebJobsStorage")] WalterEntity walterEntity,
           [Table("walter", Connection = "AzureWebJobsStorage")] CloudTable walterTable,

           string id,
           ILogger log)
        {
            log.LogInformation($"Delete Walter: {id}, received");

            if (walterEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Walter not found"
                });
            }

            await walterTable.ExecuteAsync(TableOperation.Delete(walterEntity));
            string message = $"Walter: {walterEntity.RowKey}, deleted";
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
