using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Context.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonCardCatalogue.Common.Constants;
using PokemonCardCatalogue.Common.Models;

namespace PokemonCardCatalogue.Common.Context
{
    public class PokemonTcgApi : IApi
    {
        private static ICache _cache 
            = new DbCache(new SqliteDatabaseService());
        
        private readonly IApiService _apiService;

        private static PokemonTcgApi _instance;
        public static PokemonTcgApi Instance
        {
            get => _instance ??= new PokemonTcgApi(_cache, 
                new PokemonTcgApiService(_cache, Self.GlobalHttpClient));
        }

        public PokemonTcgApi(ICache cache,
            IApiService apiService)
        {
            _cache = cache;
            _apiService = apiService;
        }

        public async Task<Card> FetchCardAsync(QueryParameters parameters = null)
        {
            var endpoint = ApiConstants.SingleCardEndpoint;
            return await FetchAsync<Card>(endpoint, parameters);
        }

        public async Task<Set> FetchSetAsync(QueryParameters parameters = null)
        {
            var endpoint = ApiConstants.SingleSetEndpoint;
            return await FetchAsync<Set>(endpoint, parameters);
        }

        public async Task<List<Card>> GetCardsAsync(QueryParameters parameters = null)
        {
            var endpoint = ApiConstants.CardsEndpoint;
            return await GetAsync<Card>(endpoint, parameters);
        }

        public async Task<List<Set>> GetSetsAsync(QueryParameters parameters = null, bool forceWebRequest = false)
        {
            var endpoint = ApiConstants.SetsEndpoint;
            return await GetAsync<Set>(endpoint, parameters, forceWebRequest);
        }

        private async Task<T> FetchAsync<T>(string endpoint, QueryParameters parameters) where T: BaseObject
        {
            var deserializedWebResult = await _apiService.FetchAsync<T>(endpoint, parameters);

            if (deserializedWebResult is null)
            {
                return default;
            }

            return deserializedWebResult?.Data;
        }

        private async Task<List<T>> GetAsync<T>(string endpoint, QueryParameters parameters, bool forceWebRequest = false) where T : BaseObject
        {
            var deserializedWebResult = await _apiService.GetAsync<T>(endpoint, parameters, forceWebRequest);

            if (deserializedWebResult is null)
            {
                return new List<T>();
            }

            return deserializedWebResult
                ?.Data
                ?.ToList();
        }

        public Task ClearCacheAsync()
        {
            return _cache.ClearAllCacheAsync();
        }
    }
}
