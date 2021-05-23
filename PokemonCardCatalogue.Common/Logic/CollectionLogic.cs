using PokemonCardCatalogue.Common.Constants;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic
{
    public class CollectionLogic : BaseLogic, ICollectionLogic
    {
        private const int pageSizeMaximum = 250;

        private readonly ICardCollection _cardCollection;

        public CollectionLogic(IApi api,
            ICardCollection cardCollection)
            : base(api)
        {
            _cardCollection = cardCollection;
        }

        public async Task<bool> AddSetAndCardsToCollection(Set set)
        {
            var saveSetTask = _cardCollection.AddSetAsync(set);
            var getCardsTask = GetAllCardsForSet(set.Id, set.Total);

            await Task.WhenAll(saveSetTask, getCardsTask);

            var allCards = getCardsTask.Result;

            if (allCards is null)
            {
                // error
                return false;
            }

            await _cardCollection.AddCardsAsync(allCards);

            return true;
        }

        private async Task<List<Card>> GetAllCardsForSet(string setId, int setTotal)
        {
            if (setTotal <= pageSizeMaximum)
            {
                return await GetPageOfCardsForSetAsync(setId, 1);
            }

            int totalPages = (int)Math.Ceiling((double)setTotal / (double)pageSizeMaximum);

            // Fetch all pages for set as separate web reqs
            Task<List<Card>>[] setPageFetchTasks = new Task<List<Card>>[totalPages];
            for (int i = 0; i < totalPages; i++)
            {
                setPageFetchTasks[i] = GetPageOfCardsForSetAsync(setId, i + 1);
            }

            await Task.WhenAll(setPageFetchTasks);

            // when all are done, combine into single collection and return.
            var allCardsForSet = new List<Card>();
            for (int i = 0; i < setPageFetchTasks.Length; i++)
            {
                allCardsForSet.AddRange(setPageFetchTasks[i].Result);
            }

            return allCardsForSet;
        }

        private Task<List<Card>> GetPageOfCardsForSetAsync(string setId, int page)
        {

            return Api.GetCardsAsync(new QueryParameters
            {
                Query = new Dictionary<string, string>
                {
                    {  "set.id", setId }
                },
                Page = page
            });
        }
        public Task<List<SetItem>> GetAllSets(bool withCount = true)
        {
            return _cardCollection.GetSetItemsAsync(withCount);
        }
        
        public Task<int> DeleteSetAsync(Set setToDelete)
        {
            return _cardCollection.DeleteSetAndCardsAsync(setToDelete);
        }

        public Task<List<CardItem>> GetCardsForSetAsync(string setId)
        {
            return _cardCollection.GetCardItemsAsync(setId);
        }

        #region Set Owned Count

        public Task<int> QuickAddCardToCollection(CardItem cardItem)
        {
            if (Rarity.IsHolo(cardItem.Card.Rarity))
            {
                return SetHoloOwnedCountForCard(cardItem.Card.Id, cardItem.HoloOwnedCount);
            }

            return SetNormalOwnedCountForCard(cardItem.Card.Id, cardItem.NormalOwnedCount);
        }

        private Task<int> SetNormalOwnedCountForCard(string id, int ownedCount)
        {
            return SetOwnedCountForCard(id, ownedCount, Queries.SetNormalCardOwnedCountById, Queries.GetNormalCardOwnedCountById);
        }
        

        private Task<int> SetHoloOwnedCountForCard(string id, int ownedCount)
        {
            return SetOwnedCountForCard(id, ownedCount, Queries.SetHoloCardOwnedCountById, Queries.GetHoloCardOwnedCountById);
        }

        private async Task<int> SetOwnedCountForCard(string id, int ownedCount, string setQuery, string getQuery)
        {

            string cardId = id;
            _ = await _cardCollection.ExecuteAsync
            (
                setQuery,
                ownedCount,
                DateTime.UtcNow,
                cardId
            );

            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
                getQuery,
                cardId
            );

            return countFromDb;
        }

        #endregion

        #region Increment Owned Count 

        public Task<int> IncrementCardNormalOwnedCount(string id)
        {
            return IncrementOwnedCount(id, Queries.GetNormalCardOwnedCountById, Queries.SetNormalCardOwnedCountById);
        }
        
        public Task<int> IncrementCardHoloOwnedCount(string id)
        {
            return IncrementOwnedCount(id, Queries.GetHoloCardOwnedCountById, Queries.SetHoloCardOwnedCountById);
        }
        
        public Task<int> IncrementCardReverseOwnedCount(string id)
        {
            return IncrementOwnedCount(id, Queries.GetReverseCardOwnedCountById, Queries.SetReverseCardOwnedCountById);
        }

        private async Task<int> IncrementOwnedCount(string id, string getQuery, string setQuery)
        {
            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
                getQuery, id
            );
            countFromDb += 1;
            _ = await _cardCollection.ExecuteAsync
            (
                setQuery,
                countFromDb,
                DateTime.UtcNow,
                id
            );

            return countFromDb;
        }

        #endregion

        #region Decrement Owned Counts

        public Task<int> DecrementCardNormalOwnedCount(string id)
        {
            return DecrementOwnedCount(id, Queries.GetNormalCardOwnedCountById, Queries.SetNormalCardOwnedCountById);
        }
        
        public Task<int> DecrementCardHoloOwnedCount(string id)
        {
            return DecrementOwnedCount(id, Queries.GetHoloCardOwnedCountById, Queries.SetHoloCardOwnedCountById);
        }
        
        public Task<int> DecrementCardReverseOwnedCount(string id)
        {
            return DecrementOwnedCount(id, Queries.GetReverseCardOwnedCountById, Queries.SetReverseCardOwnedCountById);
        }

        private async Task<int> DecrementOwnedCount(string id, string getQuery, string setQuery)
        {
            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
               getQuery, id
            );
            countFromDb -= 1;
            _ = await _cardCollection.ExecuteAsync
            (
                setQuery,
                countFromDb,
                DateTime.UtcNow,
                id
            );

            return countFromDb;
        }

        #endregion

        #region Get Owned Count

        public Task<int> GetCardNormalOwnedCount(string cardId)
        {
            return GetCardOwnedCount(cardId, Queries.GetNormalCardOwnedCountById);
        }
        
        public Task<int> GetCardHoloOwnedCount(string cardId)
        {
            return GetCardOwnedCount(cardId, Queries.GetHoloCardOwnedCountById);
        }
        
        public Task<int> GetCardReverseOwnedCount(string cardId)
        {
            return GetCardOwnedCount(cardId, Queries.GetReverseCardOwnedCountById);
        }
        
        private Task<int> GetCardOwnedCount(string cardId, string query)
        {
            return _cardCollection.ExecuteScalarAsync<int>
            (
                query,
                cardId
            );
        }

        #endregion

        public Task<DateTime?> GetMostRecentCardModifiedDateBySetId(string setId)
        {
            return _cardCollection.ExecuteScalarAsync<DateTime?>(Queries.GetMostRecentModifiedDateBySetId, setId);
        }

        public Task<CardItem> GetMostRecentlyUpdatedCardBySetId(string setId)
        {
            return _cardCollection.FindCardByQueryAsync(Queries.GetMostRecentlyModifiedCardBySetId, setId);
        }

        public Task<float> GetEstimatedCollectionMarketValue()
        {
            return _cardCollection.ExecuteScalarAsync<float>(Queries.GetEstimatedCollectionMarketValue);
        }
    }
}
