using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Card : BaseObject
    {
        [JsonPropertyName("supertype")]
        public string Supertype { get; set; }

        [JsonPropertyName("subtypes")]
        public string[] Subtypes { get; set; }

        [JsonPropertyName("hp")]
        public string Hp { get; set; }

        [JsonPropertyName("types")]
        public string[] Types { get; set; }

        [JsonPropertyName("evolvesTo")]
        public string[] EvolvesTo { get; set; }

        [JsonPropertyName("attacks")]
        public Attack[] Attacks { get; set; }

        [JsonPropertyName("weaknesses")]
        public Weakness[] Weaknesses { get; set; }

        [JsonPropertyName("retreatCost")]
        public string[] RetreatCost { get; set; }

        [JsonPropertyName("convertedRetreatCost")]
        public int ConvertedRetreatCost { get; set; }

        [JsonPropertyName("set")]
        public Set Set { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("rarity")]
        public string Rarity { get; set; }

        [JsonPropertyName("nationalPokedexNumbers")]
        public int[] NationalPokedexNumbers { get; set; }

        [JsonPropertyName("legalities")]
        public Legalities Legalities { get; set; }

        [JsonPropertyName("images")]
        public CardImages Images { get; set; }

        [JsonPropertyName("tcgplayer")]
        public TcgPlayer TcgPlayer { get; set; }
    }
}
