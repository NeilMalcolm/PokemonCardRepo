namespace PokemonCardCatalogue.Constants
{
    public static class Queries
    {
        public const string GetAllSetItemsWithOwnedCount =
            @"SELECT cs.Id,
                     cs.Name,
                     cs.Series,
                     cs.PrintedTotal,
                     cs.Total,
                     cs.ExpandedLegality,
                     cs.UnlimitedLegality,
                     cs.PokemonTcgOnlineCode,
                     cs.ReleaseDate,
                     cs.UpdatedAt,
                     cs.LogoImage,
                     cs.SymbolImage,
                     cs.DateAdded,
                     count(cc.OwnedCount > 0) as OwnedCardsCount
            FROM CollectionSet cs
            LEFT JOIN CollectionCard cc 
	            ON cc.SetId = cs.Id
            GROUP BY cs.Id";

        public static string DeleteAllCardsForSet =
            @"DELETE FROM CollectionCard
              WHERE SetId = ?";

        public static string GetAllSetIdsInCollection =
            @"SELECT Id 
              FROM CollectionSet";
    }
}
