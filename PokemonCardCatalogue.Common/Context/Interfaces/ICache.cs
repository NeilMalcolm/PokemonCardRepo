using PokemonCardCatalogue.Common.Models;
using System;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context.Interfaces
{
    public interface ICache
    {
        Task ClearAllCacheAsync();
        void Init(string fileName = null, TimeSpan? defaultCacheDuration = null);
        Task<T> GetAsync<T>(string endpoint, QueryParameters parameters = null) where T : new();
        Task WriteToCacheAsync(string endpoint, QueryParameters parameters, string payload, TimeSpan? cacheDuration = null);
    }
}
