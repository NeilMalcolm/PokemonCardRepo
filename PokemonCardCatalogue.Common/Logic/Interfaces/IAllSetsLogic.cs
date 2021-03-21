using PokemonCardCatalogue.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface IAllSetsLogic
    {
        Task<List<ApiSetItem>> GetSetsAsync();
        Task<List<ApiSetItem>> GetSetsOrderedByMostRecentAsync();
        Task<List<string>> GetSetIdsInCollectionAsync();
    }
}
