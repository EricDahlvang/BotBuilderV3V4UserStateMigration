using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Storage.Migration.CosmosDb
{
    public class CosmosDbDocumentConverter
    {
        public delegate void GetPropertiesDelegate(BotData botData, ConcurrentDictionary<string, object> dictionary);
        public GetPropertiesDelegate GetProperties = null;

        private readonly V3CosmosDbStorageOptions _v3StorageOptions;

        string[] _processChannels { get; set; } = new[] {Channels.Console, Channels.Cortana, Channels.Directline,
                                            Channels.Email, Channels.Emulator, Channels.Facebook,
                                            Channels.Groupme, Channels.Kik, Channels.Line,
                                            Channels.Msteams, Channels.Skype, Channels.Skypeforbusiness,
                                            Channels.Slack, Channels.Sms, Channels.Telegram, Channels.Webchat};

        public CosmosDbDocumentConverter(V3CosmosDbStorageOptions v3StorageOptions, string[] channels = null)
        {
            _v3StorageOptions = v3StorageOptions;
            if (channels != null)
            {
                _processChannels = channels;
            }
        }

        /// <summary>
        /// Connects to CosmosDb, loops through all documents 100 at a time and calls GetProperties
        /// for every UserData record.  If any properties are converted, they are stored into the
        /// new V4 CosmosDbStorage with the new UserData key.
        /// </summary>
        public async Task ConvertDocuments(IStorage newStorage)
        {
            if (GetProperties == null)
                throw new InvalidOperationException("GetProperties must be implemented for migration");
            
            using (var client = new DocumentClient(new Uri(_v3StorageOptions.Endpoint), _v3StorageOptions.AuthKey))
            {
                var collection = UriFactory.CreateDocumentCollectionUri(_v3StorageOptions.Database, _v3StorageOptions.Collection);
                var options = new FeedOptions { MaxItemCount = 100 };
                using (var query = client.CreateDocumentQuery<Document>(collection, options).AsDocumentQuery())
                {
                    while (query.HasMoreResults)
                    {
                        var results = await query.ExecuteNextAsync<Document>();
                        await ConvertDocuments(newStorage, results);
                    }
                }
            }
        }

        /// <summary>
        /// This is a helper method that attempts to pull a typed property from a V3 BotData object
        /// and add it to a ConcurrentDictionary to be saved into V4 cosmos.
        /// </summary>
        /// <typeparam name="T">The Type of object stored in Cosmos</typeparam>
        /// <param name="botData">V3 BotData object, retrieved from a V3 CosmosDb record</param>
        /// <param name="values">The dictionary of converted Properties (this will be stored in V4 CosmosDb)</param>
        /// <param name="v3PropertyName">The name of the property in V3 CosmosDb record.</param>
        /// <param name="v4PropertyName">The name of the property to be stored in V4 CosmosDb record.</param>
        public static void AddPropertyIfExists<T>(BotData botData,
                            ConcurrentDictionary<string, object> values,
                            string v3PropertyName,
                            string v4PropertyName)
        {
            // Retreive the property from the v3 BotData bag
            var property = botData.GetProperty<T>(v3PropertyName);
            // If the property is present, add it to the ConcurrentDictionary to
            // be serialized into V4 state
            if (property != null)
                values.TryAdd(v4PropertyName, property);
        }

        private async Task ConvertDocuments(IStorage newStorage, FeedResponse<Document> results)
        {
            foreach (var doc in results)
            {
                DocDbBotDataEntity entity = (dynamic)doc.ToString();
                var botData = new BotData()
                {
                    Data = entity.Data,
                    ETag = doc.ETag
                };

                var newId = GetNewId(doc.Id);
                // only process records where the id is changed
                // (GetNewId will return empty if the record is not supposed to be migrated)
                if (!string.IsNullOrEmpty(newId))
                {
                    var properties = new ConcurrentDictionary<string, object>();
                    GetProperties(botData, properties);

                    if (properties.Count > 0)
                    {
                        // change the Id to v4 format, and send the properties to storage to be
                        // persisted in the new collection in v4 serialization format
                        var dictionary = new Dictionary<string, object>();
                        dictionary.Add(newId, properties);
                        await newStorage.WriteAsync(dictionary, CancellationToken.None);
                    }
                }
            }
        }

        private string GetNewId(string id)
        {
            // loop through all channels, replacing key values
            foreach (var channel in _processChannels)
            {
                // v3 format: {channelId}:user{userId}
                // v4 format: {channelId}/users/{userId}
                if (id.StartsWith($"{channel}:user"))
                {
                    var newId = ReplaceChannelKey(id, channel, "user", "users");
                    if (!newId.Equals(id))
                        return newId;
                }
            }
            return string.Empty;
        }

        private static string ReplaceChannelKey(string theString, string channel, string oldKey, string newKey)
        {
            return theString.Replace(channel + ":" + oldKey, channel + "/" + newKey + "/");
        }
    }
}
