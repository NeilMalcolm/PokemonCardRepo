using PokemonCardCatalogue.Services.Interfaces;
using LightInject;
using System;

namespace PokemonCardCatalogue.Services
{
    public class DependencyContainer : IDependencyContainer
    {
        private static IServiceContainer container;

        public void Init()
        {
            container = new ServiceContainer();
        }

        public T Get<T>() where T : class
        {
            return container.GetInstance<T>();
        }

        public object Get(Type type)
        {
            return container.GetInstance(type);
        }
        public void Register<TInterface, TConcrete>(bool singleton)
            where TInterface : class
            where TConcrete : TInterface
        {
            if (singleton)
            {
                container.RegisterSingleton<TInterface, TConcrete>();
            }
            else
            {
                container.Register<TInterface, TConcrete>();
            }
        }

        public void Register<TInterface>(TInterface instance) where TInterface : class
        {
            container.Register<TInterface>((a) => instance);
        }

        public void Register<TConcrete>() where TConcrete : class
        {
            container.Register<TConcrete>();
        }
    }
}
