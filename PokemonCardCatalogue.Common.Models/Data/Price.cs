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

        private float? _highestPrice;
        public float HighestPrice 
        {
            get
            {
                return _highestPrice ??= Math.Max
                (
                    Holofoil?.High.Value ?? 0f,
                    Math.Max
                    (
                        ReverseHolofoil?.High.Value ?? 0f,
                        Normal?.High.Value ?? 0f
                    )
                );
            }
        }

        private float? _lowestPrice;
        public float LowestPrice
        {
            get
            {
                return _lowestPrice ??= Math.Min
                (
                    Holofoil?.Low.Value ?? float.MaxValue,
                    Math.Min
                    (
                        ReverseHolofoil?.Low.Value ?? float.MaxValue,
                        Normal?.Low.Value ?? float.MaxValue
                    )
                );
            }
        }
    }
}
