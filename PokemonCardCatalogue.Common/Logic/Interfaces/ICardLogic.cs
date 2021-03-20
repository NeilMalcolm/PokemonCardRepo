using PokemonCardCatalogue.Common.Models.Data;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Common.Logic.Interfaces
{
    public interface ICardLogic
    {
        Task<Card> GetCardDetails();
    }
}
