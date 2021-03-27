using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic.Interfaces
{
    public interface ISetListLogic
    {
        Task<List<Card>> GetAllCardsForSetAsync(string setId);
        bool DoSetsAndSetFromDbHaveDifferentOwnedCounts(List<SetItem> setItems, SetItem set);
    }
}
