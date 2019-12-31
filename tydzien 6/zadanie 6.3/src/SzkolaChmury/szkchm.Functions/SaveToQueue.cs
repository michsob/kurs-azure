using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace szkchm.Functions
{
    public static class SaveToQueue
    {
        [FunctionName("SaveToQueue")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, [Queue("custommsg", Connection = "AzureWebJobsStorage")] out string queueMessage, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            queueMessage = $"Custom Message - {DateTime.Now}";
        }
    }
}
