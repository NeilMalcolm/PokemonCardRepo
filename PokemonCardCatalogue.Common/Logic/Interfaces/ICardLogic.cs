using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface ICardLogic
    {
        Task<Card> GetCardDetails();
        Task<List<Card>> GetRelatedCardsInSetAsync(Card card);
    }
}
