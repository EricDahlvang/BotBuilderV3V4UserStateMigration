using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bot.Builder.Storage.Migration.CosmosDb
{
    public partial class BotData
    {
        /// <summary>
        /// Get a property from a BotData recorded retrieved using the REST API
        /// </summary>
        /// <param name="property">property name to change</param>
        /// <returns>property requested or default for type</returns>
        public TypeT GetProperty<TypeT>(string property)
        {
            if (this.Data == null)
                this.Data = new JObject();

            dynamic data = this.Data;
            if (data[property] == null)
                return default(TypeT);

            // convert jToken (JArray or JObject) to the given typeT
            return (TypeT)(data[property].ToObject(typeof(TypeT)));
        }


        /// <summary>
        /// Set a property on a BotData record retrieved using the REST API
        /// </summary>
        /// <param name="property">property name to change</param>
        /// <param name="data">new data</param>
        public void SetProperty<TypeT>(string property, TypeT data)
        {
            if (this.Data == null)
                this.Data = new JObject();

            // convert (object or array) to JToken (JObject/JArray)
            if (data == null)
                ((JObject)this.Data)[property] = null;
            else
                ((JObject)this.Data)[property] = JToken.FromObject(data);
        }

        /// <summary>
        /// Remove a property from the BotData record
        /// </summary>
        /// <param name="property">property name to remove</param>
        public void RemoveProperty(string property)
        {
            if (this.Data == null)
                this.Data = new JObject();

            ((JObject)this.Data).Remove(property);
        }
    }
    public partial class BotData
    {
        /// <summary>
        /// Initializes a new instance of the BotData class.
        /// </summary>
        public BotData()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BotData class.
        /// </summary>
        public BotData(string eTag = default(string), object data = default(object))
        {
            Data = data;
            ETag = eTag;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "eTag")]
        public string ETag { get; set; }

    }
}
