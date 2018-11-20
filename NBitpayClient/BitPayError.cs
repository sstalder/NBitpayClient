using Newtonsoft.Json;

namespace NBitpayClient
{
    public class BitPayError
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}