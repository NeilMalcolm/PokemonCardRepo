using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Helpers;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Logic
{
    public class CardLogic : BaseLogic, ICardLogic
    {
        public CardLogic(IApi api) 
            : base(api)
        {
        }

        public Task<Card> GetCardDetails()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Card>> GetRelatedCardsInSetAsync(Card card)
        {
            if (card is null)
            {
                return Task.FromResult(new List<Card>());
            }

            if (card.Supertype == Supertypes.Pokemon)
            {
                return GetOtherCardsForPokemonInSet(card);
            }

            return GetOtherCardsForNonPokemonCardInSet(card);
        }

        private Task<List<Card>> GetOtherCardsForPokemonInSet(Card card)
        {
            return Api.GetCardsAsync(new Common.Models.QueryParameters
            {
                Query = new Dictionary<string, string>(BuildOtherCardsInSetQuery(card)),
                OrderBy = "number"
            });
        }

        private Task<List<Card>> GetOtherCardsForNonPokemonCardInSet(Card card)
        {
            return Api.GetCardsAsync(new Common.Models.QueryParameters
            {
                Query = new Dictionary<string, string>
                {
                    { "name", QueryHelper.GetCardBaseName(card.Name) },
                    { "-id", card.Id },
                    { "set.id", card.Set.Id }
                },
                OrderBy = "number"
            });
        }

        private List<KeyValuePair<string, string>> BuildOtherCardsInSetQuery(Card card)
        {
            var baseQuery = QueryHelper.GetPokedexNumberQuery(card.NationalPokedexNumbers);

            if (baseQuery.Count == 0)
            {
                baseQuery.Add(new KeyValuePair<string, string>
                (
                    "name", 
                    QueryHelper.GetCardBaseName(card.Name))
                );
            }

            baseQuery.Add(new KeyValuePair<string, string>("set.id", card.Set.Id));
            baseQuery.Add(new KeyValuePair<string, string>("-id", card.Id));

            return baseQuery;
        }
    }
}
