using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
