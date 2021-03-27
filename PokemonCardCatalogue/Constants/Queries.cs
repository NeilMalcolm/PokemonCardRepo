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

        public const string DeleteAllCardsForSet =
            @"DELETE FROM CollectionCard
              WHERE SetId = ?";

        public const string GetAllSetIdsInCollection =
            @"SELECT Id 
              FROM CollectionSet";

        public const string SetCardOwnedCountById =
             @"UPDATE CollectionCard 
              SET OwnedCount = ?, 
                  ModifiedDate = ? 
              WHERE Id = ?";

        public const string GetCardOwnedCountById =
            @"SELECT OwnedCount 
              FROM CollectionCard 
              WHERE Id = ?
              LIMIT 1";

        public const string GetMostRecentModifiedDateBySetId =
            @"SELECT MAX(ModifiedDate) 
              FROM CollectionCard 
              WHERE SetId = ?";

        public const string GetMostRecentlyModifiedCardBySetId =
            @"SELECT * 
                FROM CollectionCard 
                WHERE SetId = ? 
                ORDER BY ModifiedDate DESC 
                LIMIT 1";
    }
}
