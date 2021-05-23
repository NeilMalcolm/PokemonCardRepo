using SQLite;
using System;

namespace PokemonCardCatalogue.Models.Collection
{
    public class CollectionCard : BaseCollectionItem
    {
        public string Supertype { get; set; }
        public string Hp { get; set; }
        public int ConvertedRetreatCost { get; set; }

        [Indexed]
        public string SetId { get; set; }
        public string Number { get; set; }
        public string Artist { get; set; }
        public string Rarity { get; set; }
        public string LargeImage { get; set; }
        public string SmallImage { get; set; }
        public string TcgPlayerUrl { get; set; }

        public int NormalOwnedCount { get; set; }
        public int HoloOwnedCount { get; set; }
        public int ReverseHoloOwnedCount { get; set; }

        public DateTime? AddedDate { get; set; }


        public float? FirstEditionHolofoilLow { get; set; }
        public float? FirstEditionHolofoilMid { get; set; }

        public float? FirstEditionHolofoilHigh { get; set; }

        [Indexed]
        public float? FirstEditionHolofoilMarket { get; set; }
        public float? FirstEditionHolofoilDirectLow { get; set; }


        public float? HolofoilLow { get; set; }
        public float? HolofoilMid { get; set; }

        public float? HolofoilHigh { get; set; }

        [Indexed]
        public float? HolofoilMarket { get; set; }
        public float? HolofoilDirectLow { get; set; }

        public float? NormalLow { get; set; }
        public float? NormalMid { get; set; }

        public float? NormalHigh { get; set; }

        [Indexed]
        public float? NormalMarket { get; set; }
        public float? NormalDirectLow { get; set; }

        public float? ReverseHolofoilLow { get; set; }
        public float? ReverseHolofoilMid { get; set; }

        public float? ReverseHolofoilHigh { get; set; }

        [Indexed]
        public float? ReverseHolofoilMarket { get; set; }
        public float? ReverseHolofoilDirectLow { get; set; }

        public string UnlimitedLegality { get; set; }
        public string ExpandedLegality { get; set; }
        public int NationalPokedexNumbersString { get; set; }
        public string AttacksString { get; set; }
        public string WeaknessesString { get; set; }
        public string RetreatCostString { get; set; }
        public string SubtypesString { get; set; }
        public string TypesString { get; set; }
        public string EvolvesToString { get; set; }
    }
}
