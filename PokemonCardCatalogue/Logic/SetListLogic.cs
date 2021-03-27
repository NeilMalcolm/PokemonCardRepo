using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PokemonCardCatalogue.Logic
{
    public class SetListLogic : BaseLogic, ISetListLogic
    {
        private const string SetIdParameter = "set.id";
        public SetListLogic(IApi api)
            : base(api)
        {
        }

        public Task<List<Card>> GetAllCardsForSetAsync(string setId)
        {
            return Api.GetCardsAsync(new QueryParameters
            {
                Query = new Dictionary<string, string>
                {
                    { SetIdParameter, setId }
                },
                OrderBy = "number"
            });
        }

        public bool DoSetsAndSetFromDbHaveDifferentOwnedCounts(List<SetItem> setItems, SetItem set)
        {
            if (setItems?.Count == 0 || set is null)
            {
                return true;
            }

            var matchingItem = setItems?.FirstOrDefault(x => x.Set.Id == set.Set.Id);
            if (matchingItem is null)
            {
                return true;
            }

            if (set.OwnedCount != matchingItem.OwnedCount)
            {
                return true;
            }

            return false;
        }
    }
}
