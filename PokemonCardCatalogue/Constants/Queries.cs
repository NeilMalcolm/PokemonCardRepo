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
                     (
                        SELECT COUNT() 
                        FROM CollectionCard cc
                        WHERE cc.SetId = cs.Id 
                        AND cc.OwnedCount > 0
                      ) as OwnedCardsCount
            FROM CollectionSet cs";

        public static string DeleteAllCardsForSet =
            @"DELETE FROM CollectionCard
              WHERE SetId = ?";

        public static string GetAllSetIdsInCollection =
            @"SELECT Id 
              FROM CollectionSet";
    }
}
