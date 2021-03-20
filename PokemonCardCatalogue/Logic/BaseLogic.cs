using PokemonCardCatalogue.Common.Context.Interfaces;

namespace PokemonCardCatalogue.Logic
{
    public abstract class BaseLogic
    {
        protected readonly IApi Api;

        public BaseLogic(IApi api)
        {
            Api = api;
        }
    }
}
