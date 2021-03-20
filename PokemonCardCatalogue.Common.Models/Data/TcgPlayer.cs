using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class TcgPlayer
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("prices")]
        public Price Prices { get; set; }
    }
}
