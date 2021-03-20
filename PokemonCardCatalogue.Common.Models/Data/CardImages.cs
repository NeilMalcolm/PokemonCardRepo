using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class CardImages
    {
        [JsonPropertyName("small")]
        public string Small { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }
    }
}
