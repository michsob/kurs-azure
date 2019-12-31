using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using szkchm.Functions.Models;

namespace szkchm.Functions.Repositories
{
    public class MessageRepository
    {
        private readonly CosmosClient cosmosClient;
        private Database database;
        private Container messagesContainer;
        private readonly string databaseName;

        public MessageRepository(CosmosClient cosmosClient, string databaseName)
        {
            this.cosmosClient = cosmosClient ?? throw new ArgumentNullException("cosmosClient");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException("databaseName");

            this.databaseName = databaseName;
        }

        public async Task InitializeDatabase()
        {
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName, 400);

            messagesContainer = await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = "messages",
                PartitionKeyPath = "/id"
            });
        }

        public async Task<List<Message>> GetMessages(DateTime date)
        {
            await InitializeDatabase();

            var messages = new List<Message>();
            var querable = messagesContainer.GetItemLinqQueryable<Message>(requestOptions: new QueryRequestOptions
            {
                MaxItemCount = 100
            });

            var setIterator = querable.Where(c => c.Date == date.ToString("yyyy-MM-dd")).ToFeedIterator();

            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                    messages.Add(item);
            }

            return messages;
        }

        public async Task<Message> CreateMessage(string text, DateTime date)
        {
            await InitializeDatabase();

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                Text = text,
                Date = date.ToString("yyyy-MM-dd")
            };

            return await messagesContainer.CreateItemAsync<Message>(message, new PartitionKey(message.Id));
        }
    }
}
