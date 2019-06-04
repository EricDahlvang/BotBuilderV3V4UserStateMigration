
namespace V4CosmosDbStateBot
{
    using Bot.Builder.Storage.Migration.CosmosDb;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;
    using V4CosmosDbStateBot.Bots;
    using static V4CosmosDbStateBot.Bots.EchoBot;

    class Program
    {
        static void Main(string[] args)
        {
            // Retreive connection settins from appSettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appSettings.json", optional: true);

            IConfigurationRoot configuration = builder.Build();

            // Create storage options required to connect to CosmosDb for V3
            var v3Options = new V3CosmosDbStorageOptions(configuration["v3CosmosEndpoint"],
                                                configuration["v3CosmosKey"],
                                                configuration["v3CosmosDatabase"],
                                                configuration["v3CosmosCollection"]);
            
            // The CosmosDbDocumentConverter will connect to V3 Cosmos storage, loop through every UserData
            // record and call GetProperties, sending a v3 BotData object and ConcurrentDictionary within 
            // which the V4 converted records should be placed.
            var converter = new CosmosDbDocumentConverter(v3Options);

            converter.GetProperties = (botData, properties) =>
            {
                CosmosDbDocumentConverter.AddPropertyIfExists<TestDataClass>(botData, properties, "V3TestDataClass", "V4TestDataClass");
                CosmosDbDocumentConverter.AddPropertyIfExists<GreetingState>(botData, properties, "V3TestGreeting", "V4TestGreeting");
                CosmosDbDocumentConverter.AddPropertyIfExists<string>(botData, properties, "test", "test");
                CosmosDbDocumentConverter.AddPropertyIfExists<bool>(botData, properties, "AskedName", "AskedName");
            };

            var newStorage = new CosmosDbStorage(new CosmosDbStorageOptions()
                                {
                                    AuthKey = configuration["v4CosmosKey"],
                                    DatabaseId = configuration["v4CosmosDatabase"],
                                    CollectionId = configuration["v4CosmosCollection"],
                                    CosmosDBEndpoint = new Uri(configuration["v4CosmosEndpoint"])
                                });

            converter.ConvertDocuments(newStorage).Wait();
        }
    }
}