using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface IAllSetsLogic
    {
        Task<List<ApiSetItem>> GetSetsAsync();
        Task<Set> GetSetByIdAsync(string id);
        Task<List<ApiSetItem>> GetSetsOrderedByMostRecentAsync();
        Task<List<string>> GetSetIdsInCollectionAsync();
    }
}
