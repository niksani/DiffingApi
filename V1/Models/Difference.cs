using Newtonsoft.Json;

namespace DiffingApi.V1.Models
{
    public class Difference
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }
    }
}