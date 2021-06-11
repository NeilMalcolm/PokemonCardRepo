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

        private float? highestPrice;
        public float? HighestPrice
        {
            get => highestPrice ??=
            (
                Math.Max(
                    Math.Max(Normal?.High ?? 0, ReverseHolofoil?.High ?? 0),
                    Math.Max(Holofoil?.High ?? 0, FirstEditionHolofoil?.High ?? 0)
                )
            );
        }
        private float? lowestPrice;
        public float? LowestPrice
        {
            get => lowestPrice ??=
            (
                Math.Min(
                    Math.Min(Normal?.Low ?? float.MaxValue, ReverseHolofoil?.Low ?? float.MaxValue),
                    Math.Min(Holofoil?.Low ?? float.MaxValue, FirstEditionHolofoil?.Low ?? float.MaxValue)
                )
            );
        }
    }
}
