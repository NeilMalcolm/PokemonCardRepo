using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Models.Collection;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic
{
    public class AllSetsLogic : BaseLogic, IAllSetsLogic
    {
        private readonly ICardCollection _cardCollection;

        public AllSetsLogic(IApi api,
            ICardCollection cardCollection) 
            : base(api)
        {
            _cardCollection = cardCollection;
        }

        public async Task<List<ApiSetItem>> GetSetsAsync()
        {
            return await GetApiSetItems(Api.GetSetsAsync());
        }

        public async Task<List<ApiSetItem>> GetSetsOrderedByMostRecentAsync()
        {
            return await GetApiSetItems(Api.GetSetsAsync(new QueryParameters
            {
                OrderBy = "-releaseDate"
            }));
        }

        private async Task<List<ApiSetItem>> GetApiSetItems(Task<List<Set>> apiTask)
        {
            var idsInCollectionTask = GetSetIdsInCollectionAsync();
            
            await Task.WhenAll(apiTask, idsInCollectionTask);

            var apiResult = apiTask.Result;
            var idsInCollection = idsInCollectionTask.Result;

            return apiResult
                .Select(s => new ApiSetItem
                {
                    Set = s,
                    IsInCollection = idsInCollection?.Any(x => x == s.Id) ?? false
                })
                .ToList();
        }

        public async Task<List<string>> GetSetIdsInCollectionAsync()
        {
            var results = await _cardCollection.QueryAsync<IdResult>(Queries.GetAllSetIdsInCollection);
            return results.Select(x => x.Id)
                .ToList();
        }
    }
}
