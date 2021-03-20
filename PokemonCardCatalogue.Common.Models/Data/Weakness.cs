using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Weakness
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
