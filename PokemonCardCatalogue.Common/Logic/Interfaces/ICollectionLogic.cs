using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface ICollectionLogic
    {
        Task<bool> AddSetAndCardsToCollection(Set set);
        Task<List<SetItem>> GetAllSets(bool withCount = true);
        Task<int> DeleteSetAsync(Set setToDelete);
        Task<List<CardItem>> GetCardsForSetAsync(string setId);
        Task<int> QuickAddCardToCollection(CardItem cardItem);
        Task<int> IncrementCardNormalOwnedCount(string cardId);
        Task<int> IncrementCardHoloOwnedCount(string cardId);
        Task<int> IncrementCardReverseOwnedCount(string cardId);
        Task<int> DecrementCardNormalOwnedCount(string cardId);
        Task<int> DecrementCardHoloOwnedCount(string cardId);
        Task<int> DecrementCardReverseOwnedCount(string cardId);
        Task<OwnedCounter> GetCardOwnedCounts(string cardId);
        Task<DateTime?> GetMostRecentCardModifiedDateBySetId(string setId);
        Task<IList<CardItem>> GetMostRecentlyUpdatedCardsBySetId(string setId, DateTime changesSinceDateTime);
        Task<float> GetEstimatedCollectionMarketValue();
    }
}
