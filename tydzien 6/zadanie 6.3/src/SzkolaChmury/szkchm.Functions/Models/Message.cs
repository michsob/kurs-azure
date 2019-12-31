using Newtonsoft.Json;

namespace szkchm.Functions.Models
{
    public class Message
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
    }
}
