using System.Text.Json.Serialization;

namespace Elfin.Types
{
    public class NekosLifeResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}