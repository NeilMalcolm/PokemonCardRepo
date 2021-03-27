using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Common.Models.Types;
using System.Collections.Generic;
using System.Linq;
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
                    { "name", GetCardBaseName(card.Name) },
                    { "-id", card.Id },
                    { "set.id", card.Set.Id }
                },
                OrderBy = "number"
            });
        }

        private List<KeyValuePair<string, string>> BuildOtherCardsInSetQuery(Card card)
        {
            var baseQuery = GetPokedexNumberQuery(card.NationalPokedexNumbers);

            if (baseQuery.Count == 0)
            {
                baseQuery.Add(new KeyValuePair<string, string>("name", GetCardBaseName(card.Name)));
            }

            baseQuery.Add(new KeyValuePair<string, string>("set.id", card.Set.Id));
            baseQuery.Add(new KeyValuePair<string, string>("-id", card.Id));

            return baseQuery;
        }

        private List<KeyValuePair<string, string>> GetPokedexNumberQuery(int[] pokedexNumbers)
        {
            if (pokedexNumbers is null)
            {
                return new List<KeyValuePair<string, string>>();
            }

            var numberQuery = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < pokedexNumbers.Length; i++)
            {
                var number = pokedexNumbers[i];
                string key = "(nationalPokedexNumbers";
                string value;

                if (i > 0 && i < pokedexNumbers.Length - 1)
                {
                    value = $"[{number} TO {number}]) or";
                }
                else
                {
                    value = $"[{number} TO {number}])";
                }

                numberQuery.Add(new KeyValuePair<string, string>(key, value));
            }

            return numberQuery;
        }

        private string GetCardBaseName(string cardName)
        {
            return $"*{cardName.Split(' ')[0]}*";
        }
    }
}
