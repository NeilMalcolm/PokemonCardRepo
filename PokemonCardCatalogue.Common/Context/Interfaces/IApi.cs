using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context.Interfaces
{
    public interface IApi
    {
        Task<List<Card>> GetCardsAsync(QueryParameters parameters = null);
        Task<Card> FetchCardAsync(QueryParameters parameters = null);
        Task<List<Set>> GetSetsAsync(QueryParameters parameters = null);
        Task<Set> FetchSetAsync(QueryParameters parameters = null);
        Task ClearCacheAsync();
    }
}
