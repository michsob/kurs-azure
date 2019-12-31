using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using szkchm.Functions.Repositories;

namespace szkchm.Functions
{
    public static class SaveToCosmos
    {
        [FunctionName("SaveToCosmos")]
        public static async Task Run([QueueTrigger("custommsg", Connection = "AzureWebJobsStorage")]string messageItem, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Queue trigger function processed: {messageItem}");

            var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cosmosClient = new CosmosClient(config["CosmosDbServiceEndpoint"], config["CosmosDbPrimaryKey"]);
            var messageRepository = new MessageRepository(cosmosClient, "szkchm");

            await messageRepository.CreateMessage(messageItem, DateTime.Now);
        }
    }
}
