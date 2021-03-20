using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class TcgPlayerCardRarityType
    {
        [JsonPropertyName("low")]
        public float? Low { get; set; }

        [JsonPropertyName("mid")]
        public float? Mid { get; set; }

        [JsonPropertyName("high")]
        public float? High { get; set; }

        [JsonPropertyName("market")]
        public float? Market { get; set; }

        [JsonPropertyName("directLow")]
        public float? DirectLow { get; set; }
    }
}
