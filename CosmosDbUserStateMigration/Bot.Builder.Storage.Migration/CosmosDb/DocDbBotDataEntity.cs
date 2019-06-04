using Newtonsoft.Json;

namespace Bot.Builder.Storage.Migration.CosmosDb
{
    internal class DocDbBotDataEntity
    {
        internal const int MAX_KEY_LENGTH = 254;
        public DocDbBotDataEntity() { }

        private static string TruncateEntityKey(string entityKey)
        {
            if (entityKey.Length > MAX_KEY_LENGTH)
            {
                var hash = entityKey.GetHashCode().ToString("x");
                entityKey = entityKey.Substring(0, MAX_KEY_LENGTH - hash.Length) + hash;
            }

            return entityKey;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "botId")]
        public string BotId { get; set; }

        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        [JsonProperty(PropertyName = "conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}
