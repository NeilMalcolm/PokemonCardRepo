using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Helpers;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context
{
    public class PokemonTcgApiService : IApiService
    {
        private readonly ICache _cache;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public PokemonTcgApiService(ICache cache)
        {
            _cache = cache;
            _cache.Init();

            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        }

        public async Task<ApiResponseDataContainer<T>> FetchAsync<T>(string endpoint, QueryParameters parameters) 
            where T : BaseObject
        {
            var cacheResult = await _cache.GetAsync<ApiResponseDataContainer<T>>(endpoint, parameters);

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var fullUrl = string.Format(endpoint, QueryHelper.BuildQuery(parameters));
            var response = await Self.GlobalClient.GetAsync(fullUrl);

            return await HandleResponse<ApiResponseDataContainer<T>>(response, endpoint, parameters);
        }

        public async Task<ApiListResponseDataContainer<T>> GetAsync<T>(string endpoint, QueryParameters parameters) 
            where T : BaseObject
        {
            var cacheResult = await _cache.GetAsync<ApiListResponseDataContainer<T>>(endpoint, parameters);

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var fullUrl = string.Format("{0}{1}", endpoint, QueryHelper.BuildQuery(parameters));
            var response = await Self.GlobalClient.GetAsync(fullUrl);

            return await HandleResponse<ApiListResponseDataContainer<T>>(response, endpoint, parameters);
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response, string endpoint, QueryParameters parameters)
        {
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            await _cache.WriteToCacheAsync(endpoint, parameters, await response.Content.ReadAsStringAsync());

            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
        }
    }
}
