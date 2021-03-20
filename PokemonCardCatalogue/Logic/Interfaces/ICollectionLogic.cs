using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic.Interfaces
{
    public interface ICollectionLogic
    {
        Task<bool> AddSetAndCardsToCollection(Set set);
        Task<List<SetItem>> GetAllSets(bool withCount = true);
        Task<int> DeleteSetAsync(Set setToDelete);
    }
}
