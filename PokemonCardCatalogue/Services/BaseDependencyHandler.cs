using PokemonCardCatalogue.Common.Context;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Logic;
using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;

namespace PokemonCardCatalogue.Services
{
    public abstract class BaseDependencyHandler : IDependencyHandler
    {
        protected readonly IDependencyContainer _dependencyContainer;

        public T Get<T>() where T : class
        {
            return _dependencyContainer.Get<T>();
        }

        public BaseDependencyHandler(IDependencyContainer dependencyContainer)
        {
            _dependencyContainer = dependencyContainer;
        }

        public void Init()
        {
            _dependencyContainer.Init();

            RegisterEarlyPlatformSpecificDependencies();
            RegisterServices();
            RegisterLatePlatformSpecificDependencies();
            RegisterLogic();
            RegisterViewModels();
        }

        public abstract void RegisterEarlyPlatformSpecificDependencies();
        public abstract void RegisterLatePlatformSpecificDependencies();

        private void RegisterServices()
        {
            _dependencyContainer.Register(_dependencyContainer);
            _dependencyContainer.Register<IApi>(PokemonTcgApi.Instance);

            _dependencyContainer.Register<IViewModelResolver, ViewModelResolver>(true);
            _dependencyContainer.Register<INavigationService, NavigationService>(true);
            _dependencyContainer.Register<IAlertService, AlertService>(true);
            _dependencyContainer.Register<ICollectionMapper, CollectionMapper>(true);
            _dependencyContainer.Register<ICardCollection, CardCollection>(true);

            _dependencyContainer.Register<IAllSetsLogic, AllSetsLogic>(true);
            _dependencyContainer.Register<ISetListLogic, SetListLogic>(true);
            _dependencyContainer.Register<ICardLogic, CardLogic>(true);
            _dependencyContainer.Register<ICollectionLogic, CollectionLogic>(true);
        }

        private void RegisterLogic()
        {
        }

        private void RegisterViewModels()
        {
            _dependencyContainer.Register<AppShellViewModel>();
            _dependencyContainer.Register<AllSetsViewModel>();
            _dependencyContainer.Register<SetListViewModel>();
            _dependencyContainer.Register<CardViewModel>();
            _dependencyContainer.Register<SettingsViewModel>();
            _dependencyContainer.Register<CollectionSetsViewModel>();
            _dependencyContainer.Register<CollectionCardListViewModel>();
        }
    }
}
