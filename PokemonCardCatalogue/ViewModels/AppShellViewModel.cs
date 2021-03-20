using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private readonly IViewModelResolver _viewModelResolver;

        private BaseViewModel _allSetsContext;
        public BaseViewModel AllSetsContext
        {
            get => _allSetsContext;
            set
            {
                _allSetsContext = value;
                OnPropertyChanged();
            }
        }

        private BaseViewModel _settingsContext;
        public BaseViewModel SettingsContext
        {
            get => _settingsContext;
            set
            {
                _settingsContext = value;
                OnPropertyChanged();
            }
        }
        private BaseViewModel _collectionSetsContext;
        public BaseViewModel CollectionSetsContext
        {
            get => _collectionSetsContext;
            set
            {
                _collectionSetsContext = value;
                OnPropertyChanged();
            }
        }

        public AppShellViewModel(IViewModelResolver viewModelResolver,
            INavigationService navigationService)
            : base (navigationService)
        {
            _viewModelResolver = viewModelResolver;
        }

        protected override Task OnLoadAsync()
        {
            AllSetsContext = _viewModelResolver.Get<AllSetsPage>();
            SettingsContext = _viewModelResolver.Get<SettingsPage>();
            CollectionSetsContext = _viewModelResolver.Get<CollectionSetsPage>();

            return Task.CompletedTask;
        }
    }
}
