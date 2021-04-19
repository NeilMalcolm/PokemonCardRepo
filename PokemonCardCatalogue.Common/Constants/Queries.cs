namespace PokemonCardCatalogue.Common.Constants
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

        public const string GetAllCardTotals =
            @"select TOTAL(NormalHigh) as NormalHighTotal,
		             TOTAL(ReverseHolofoilHigh) as ReverseHolofoilHighTotal,
		             TOTAL(HolofoilHigh) as HolofoilHighTotal
            FROM CollectionCard";

        public const string GetAllCardMarketTotals =
            @"select TOTAL(NormalMarket) as NormalMarketTotal,
		             TOTAL(HolofoilMarket) as HolofoilMarketTotal,
		             TOTAL(ReverseHolofoilMarket) as ReverseHolofoilMarketTotal
            FROM CollectionCard";

        public const string GetMaxCardMarketTotals =
            @"select TOTAL(Max(IFNULL(NormalMarket, 0.0), IFNULL(HolofoilMarket, 0.0), IFNULL(ReverseHolofoilMarket, 0.0)))
            FROM CollectionCard
            WHERE OwnedCount > 0";
    }
}
