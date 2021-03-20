using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Services.Interfaces;
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
    }
}
