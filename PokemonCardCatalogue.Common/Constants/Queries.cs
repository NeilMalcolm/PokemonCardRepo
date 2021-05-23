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
                        AND (
                            cc.NormalOwnedCount > 0 
                            OR cc.HoloOwnedCount > 0 
                            OR cc.ReverseHoloOwnedCount > 0
                        )
                      ) as OwnedCardsCount
            FROM CollectionSet cs";

        public const string DeleteAllCardsForSet =
            @"DELETE FROM CollectionCard
              WHERE SetId = ?";

        public const string GetAllSetIdsInCollection =
            @"SELECT Id 
              FROM CollectionSet";

        public const string SetNormalCardOwnedCountById =
             @"UPDATE CollectionCard 
              SET NormalOwnedCount = ?, 
                  ModifiedDate = ? 
              WHERE Id = ?";

        public const string SetHoloCardOwnedCountById =
             @"UPDATE CollectionCard 
              SET HoloOwnedCount = ?, 
                  ModifiedDate = ? 
              WHERE Id = ?";

        public const string SetReverseCardOwnedCountById =
             @"UPDATE CollectionCard 
              SET ReverseHoloOwnedCount = ?, 
                  ModifiedDate = ? 
              WHERE Id = ?";

        public const string GetNormalCardOwnedCountById =
            @"SELECT NormalOwnedCount 
              FROM CollectionCard 
              WHERE Id = ?
              LIMIT 1";

        public const string GetHoloCardOwnedCountById =
            @"SELECT HoloOwnedCount 
              FROM CollectionCard 
              WHERE Id = ?
              LIMIT 1";

        public const string GetReverseCardOwnedCountById =
            @"SELECT ReverseHoloOwnedCount 
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

        public const string GetEstimatedCollectionMarketValue =
            @"SELECT 
                TOTAL
                (
	                CASE 
		                WHEN Rarity = 'Rare ACE'  
			                OR rarity = 'Rare Holo EX' 
			                OR rarity = 'Rare Holo GX' 
			                OR rarity = 'Rare Holo V' 
			                OR rarity = 'Rare Holo VMAX' 
			                OR rarity = 'Rare Rainbow'
			                OR rarity = 'Rare Prime'
			                OR rarity = 'Rare Prism Star'
			                OR rarity = 'Rare Rainbow'
			                OR rarity = 'Rare Secret'
			                OR rarity = 'Rare Shining'
			                OR rarity = 'Rare Shiny'
			                OR rarity = 'Rare Shiny GX'
			                OR rarity = 'Rare Ultra'
			                OR rarity = 'Amazing Rare'
				                THEN (IFNULL (HolofoilMarket, 0.0) * HoloOwnedCount) -- If holo and non-holo unavailable, take holofoil market value
		                WHEN (rarity = 'Common'
			                OR rarity = 'Uncommon')
				                THEN (IFNULL (NormalMarket, 0.0) * NormalOwnedCount)
                                      + (IFNULL(ReverseHolofoilMarket, 0.0) * ReverseHoloOwnedCount)
		                WHEN rarity = 'Rare'
				                THEN (IFNULL (HolofoilMarket, 0.0) * HoloOwnedCount)
                                      + (IFNULL(ReverseHolofoilMarket, 0.0) * ReverseHoloOwnedCount)
		                WHEN rarity = 'Rare Holo'
				                THEN (IFNULL (HolofoilMarket, 0.0) * HoloOwnedCount)
                                     + (IFNULL(ReverseHolofoilMarket, 0.0) * ReverseHoloOwnedCount)
		                WHEN rarity = 'Promo'
				                THEN (IFNULL (NormalMarket, 0.0) * NormalOwnedCount) 
                                        + (IFNULL(HolofoilMarket, 0.0) * HoloOwnedCount)
                                        + (IFNULL(ReverseHolofoilMarket, 0.0) * ReverseHoloOwnedCount) 
		                ELSE 
				                0
	                END
                ) TotalMarketValue
                FROM CollectionCard
                WHERE (HoloOwnedCount > 0
                    OR ReverseHoloOwnedCount > 0
                    OR NormalOwnedCount > 0)";
    }
}
