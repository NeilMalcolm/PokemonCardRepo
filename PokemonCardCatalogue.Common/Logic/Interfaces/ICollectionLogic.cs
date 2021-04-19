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
        Task<int> SetOwnedCountForCard(CardItem cardItem);
        Task<int> IncrementCardOwnedCount(string cardId);
        Task<int> DecrementCardOwnedCount(string cardId);
        Task<int> GetCardOwnedCount(string cardId);
        Task<DateTime?> GetMostRecentCardModifiedDateBySetId(string setId);
        Task<CardItem> GetMostRecentlyUpdatedCardBySetId(string setId);
        Task<float> GetMaxMarketValueForCollection();
    }
}
