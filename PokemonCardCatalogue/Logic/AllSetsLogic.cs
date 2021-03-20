using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic
{
    public class AllSetsLogic : BaseLogic, IAllSetsLogic
    {
        public AllSetsLogic(IApi api) 
            : base(api)
        {
        }

        public async Task<List<Set>> GetSetsAsync()
        {
            return await Api.GetSetsAsync();
        }

        public async Task<List<Set>> GetSetsOrderedByMostRecentAsync()
        {
            return await Api.GetSetsAsync(new QueryParameters
            {
                OrderBy = "-releaseDate" 
            });
        }

        public async Task<bool> AddSetToCollection(Set setToAdd)
        {
            await Task.Delay(1000);
            return true;
        }
    }
}
