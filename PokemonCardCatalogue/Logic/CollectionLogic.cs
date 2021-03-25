using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic
{
    public class CollectionLogic : BaseLogic, ICollectionLogic
    {
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
            var getCardsTask = Api.GetCardsAsync(new Common.Models.QueryParameters
            {
                Query = new Dictionary<string, string>
                {
                    { "set.id", set.Id }
                }
            });

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

        public Task<int> SetOwnedCountForCard(CardItem cardItem)
        {
            return _cardCollection.SetOwnedCountForCard(cardItem.Card.Id, cardItem.OwnedCount);
        }

        public Task<int> IncrementCardOwnedCount(string id)
        {
            return _cardCollection.IncrementOwnedCountForCard(id);
        }

        public Task<int> DecrementCardOwnedCount(string id)
        {
            return _cardCollection.DecrementOwnedCountForCard(id);
        }

        public Task<int> GetCardOwnedCount(string cardId)
        {
            return _cardCollection.GetCardOwnedCount(cardId);
        }

        public Task<DateTime?> GetMostRecentCardModifiedDateBySetId(string setId)
        {
            return _cardCollection.GetMostRecentCardUpdateBySet(setId);
        }

        public Task<CardItem> GetMostRecentlyUpdatedCardBySetId(string setId)
        {
            return _cardCollection.GetMostRecentlyUpdatedCardBySetId(setId);
        }
    }
}
