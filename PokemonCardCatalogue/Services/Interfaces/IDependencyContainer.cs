using System;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface IDependencyContainer
    {
        void Init();
        public T Get<T>() where T : class;
        public object Get(Type type);

        void Register<TInterface, TConcrete>(bool singleton) where TInterface : class
                                               where TConcrete : TInterface;
        void Register<TInterface>(TInterface instance) where TInterface : class;
        void Register<TConcrete>() where TConcrete : class;
    }
}
