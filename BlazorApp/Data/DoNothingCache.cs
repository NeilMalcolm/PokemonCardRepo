using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Models;
using System;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class DoNothingCache : ICache
    {
        public Task ClearAllCacheAsync()
        {
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string endpoint, QueryParameters parameters = null) where T : new()
        {
            return Task.FromResult(default(T));
        }

        public void Init(string fileName = null, TimeSpan? defaultCacheDuration = null)
        {

        }

        public Task WriteToCacheAsync(string endpoint, QueryParameters parameters, string payload, TimeSpan? cacheDuration = null)
        {
            return Task.CompletedTask;
        }
    }
}
