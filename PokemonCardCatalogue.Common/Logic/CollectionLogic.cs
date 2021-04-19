using PokemonCardCatalogue.Common.Constants;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
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

        public async Task<int> SetOwnedCountForCard(CardItem cardItem)
        {
            string cardId = cardItem.Card.Id;
            _ = await _cardCollection.ExecuteAsync
            (
               Queries.SetCardOwnedCountById,
                cardItem.OwnedCount,
                DateTime.UtcNow,
                cardId
            );

            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
                Queries.GetCardOwnedCountById,
                cardId
            );

            return countFromDb;
        }

        public async Task<int> IncrementCardOwnedCount(string id)
        {
            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
                Queries.GetCardOwnedCountById, id
            );
            countFromDb += 1;
            _ = await _cardCollection.ExecuteAsync
            (
                Queries.SetCardOwnedCountById,
                countFromDb,
                DateTime.UtcNow,
                id
            );

            return countFromDb;
        }

        public async Task<int> DecrementCardOwnedCount(string id)
        {
            var countFromDb = await _cardCollection.ExecuteScalarAsync<int>
            (
               Queries.GetCardOwnedCountById, id
            );
            countFromDb -= 1;
            _ = await _cardCollection.ExecuteAsync
            (
                Queries.SetCardOwnedCountById,
                countFromDb,
                DateTime.UtcNow,
                id
            );

            return countFromDb;
        }

        public Task<int> GetCardOwnedCount(string cardId)
        {
            return _cardCollection.ExecuteScalarAsync<int>
            (
                Queries.GetCardOwnedCountById,
                cardId
            );
        }

        public Task<DateTime?> GetMostRecentCardModifiedDateBySetId(string setId)
        {
            return _cardCollection.ExecuteScalarAsync<DateTime?>(Queries.GetMostRecentModifiedDateBySetId, setId);
        }

        public Task<CardItem> GetMostRecentlyUpdatedCardBySetId(string setId)
        {
            return _cardCollection.FindCardByQueryAsync(Queries.GetMostRecentlyModifiedCardBySetId, setId);
        }

        public Task<float> GetMaxMarketValueForCollection()
        {
            return _cardCollection.ExecuteScalarAsync<float>(Queries.GetMaxCardMarketTotals);
        }
    }
}
