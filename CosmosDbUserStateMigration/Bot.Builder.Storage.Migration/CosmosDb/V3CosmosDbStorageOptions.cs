namespace Bot.Builder.Storage.Migration.CosmosDb
{
    public class V3CosmosDbStorageOptions
    {
        public V3CosmosDbStorageOptions(string endpoint, string authKey, string database, string collection)
        {
            Endpoint = endpoint;
            AuthKey = authKey;
            Database = database;
            Collection = collection;
        }

        public string Endpoint;
        public string AuthKey;
        public string Database;
        public string Collection;
    }
}
