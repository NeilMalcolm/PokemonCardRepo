using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PokemonCardCatalogue
{
    public partial class App : Application
    {
        private readonly IDependencyHandler _dependencyHandler;

        public App(IDependencyHandler dependencyHandler)
        {
            _dependencyHandler = dependencyHandler;
            _dependencyHandler.Init();
            Common.Self.SetApikey("8f8d3be5-5801-482e-97c0-4b7953b461fa");
            RegisterViewModels();
            InitializeComponent();

            var navigationService = _dependencyHandler.Get<INavigationService>();
            var collection = _dependencyHandler.Get<ICardCollection>();
            Task.Run(() => collection.InitAsync());
            MainPage = navigationService.GetMainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void RegisterViewModels()
        {
            var viewModelResolver = _dependencyHandler.Get<IViewModelResolver>();

            viewModelResolver.Register<AppShell, AppShellViewModel>();
            viewModelResolver.Register<AllSetsPage, AllSetsViewModel>();
            viewModelResolver.Register<SetListPage, SetListViewModel>();
            viewModelResolver.Register<CardPage, CardViewModel>();
            viewModelResolver.Register<CollectionSetsPage, CollectionSetsViewModel>();
            viewModelResolver.Register<CollectionCardListPage, CollectionCardListViewModel>();

            viewModelResolver.Register<SettingsPage, SettingsViewModel>();
        }
    }
}
