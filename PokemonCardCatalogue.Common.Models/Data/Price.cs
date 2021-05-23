using System;
using System.Text.Json.Serialization;

namespace PokemonCardCatalogue.Common.Models.Data
{
    public class Price
    {
        [JsonPropertyName("holofoil")]
        public TcgPlayerCardRarityType Holofoil { get; set; }

        [JsonPropertyName("reverseHolofoil")]
        public TcgPlayerCardRarityType ReverseHolofoil { get; set; }

        [JsonPropertyName("normal")]
        public TcgPlayerCardRarityType Normal { get; set; }
        
        [JsonPropertyName("1stEditionHolofoil")]
        public TcgPlayerCardRarityType FirstEditionHolofoil { get; set; }

    }
}
