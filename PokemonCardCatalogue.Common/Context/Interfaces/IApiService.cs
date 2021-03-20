using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Context.Interfaces
{
    public interface IApiService
    {
        Task<ApiListResponseDataContainer<T>> GetAsync<T>(string endpoint, QueryParameters parameters) where T : BaseObject;
        Task<ApiResponseDataContainer<T>> FetchAsync<T>(string endpoint, QueryParameters parameters) where T : BaseObject;
    }
}
