using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using szkchm.Functions.Repositories;

namespace szkchm.Functions
{
    public static class DisplayMessages
    {
        [FunctionName("DisplayMessages")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cosmosClient = new CosmosClient(config["CosmosDbServiceEndpoint"], config["CosmosDbPrimaryKey"]);
            var messageRepository = new MessageRepository(cosmosClient, "szkchm");

            string dateString = req.Query["date"];

            try
            {
                DateTime date = DateTime.Parse(dateString);
                log.LogInformation($"Date parameter: {date}");
                var messages = await messageRepository.GetMessages(date);

                return new JsonResult(messages);
            }
            catch(FormatException ex)
            {
                return new BadRequestObjectResult("Wrong date format");
            }
        }
    }
}
