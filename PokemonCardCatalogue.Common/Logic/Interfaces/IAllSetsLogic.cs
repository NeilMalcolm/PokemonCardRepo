using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface IAllSetsLogic
    {
        Task<List<Set>> GetSetsAsync();
        Task<List<Set>> GetSetsOrderedByMostRecentAsync();
        Task<bool> AddSetToCollection(Set setToAdd);
    }
}
