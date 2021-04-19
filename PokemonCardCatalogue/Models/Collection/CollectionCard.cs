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

        public int OwnedCount { get; set; }
        public DateTime? AddedDate { get; set; }

        public float? HolofoilLow { get; set; }
        public float? HolofoilMid { get; set; }

        [Indexed]
        public float? HolofoilHigh { get; set; }
        public float? HolofoilMarket { get; set; }
        public float? HolofoilDirectLow { get; set; }

        public float? NormalLow { get; set; }
        public float? NormalMid { get; set; }

        [Indexed]
        public float? NormalHigh { get; set; }
        public float? NormalMarket { get; set; }
        public float? NormalDirectLow { get; set; }

        public float? ReverseHolofoilLow { get; set; }
        public float? ReverseHolofoilMid { get; set; }

        [Indexed]
        public float? ReverseHolofoilHigh { get; set; }
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
