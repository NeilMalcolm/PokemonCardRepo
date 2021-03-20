using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Attack : BaseObject
    {
        [JsonPropertyName("cost")]
        public string[] Cost { get; set; }

        [JsonPropertyName("convertedEnergyCost")]
        public int ConvertedEnergyCost { get; set; }

        [JsonPropertyName("damage")]
        public string Damage { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
