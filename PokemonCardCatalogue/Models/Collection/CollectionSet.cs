using System;

namespace PokemonCardCatalogue.Models.Collection
{
    public class CollectionSet : BaseCollectionItem
    {
        public string Series { get; set; }
        public int PrintedTotal { get; set; }
        public int Total { get; set; }
        public string ExpandedLegality { get; set; }
        public string UnlimitedLegality { get; set; }
        public string PokemonTcgOnlineCode { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string UpdatedAt { get; set; }
        public string LogoImage { get; set; }
        public string SymbolImage { get; set; }
        public DateTime? DateAdded { get; set; }

        public int OwnedCardsCount { get; set; }
    }
}
