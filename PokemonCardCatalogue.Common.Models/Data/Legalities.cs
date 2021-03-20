using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Legalities
    {
        [JsonPropertyName("unlimited")]
        public string Unlimited { get; set; }

        [JsonPropertyName("expanded")]
        public string Expanded { get; set; }
    }
}
