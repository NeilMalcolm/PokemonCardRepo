using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
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
    }
}
