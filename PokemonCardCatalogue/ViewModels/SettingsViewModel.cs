using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Models.Settings;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IApi _api;
        private readonly IAlertService _alertService;
        private readonly ICardCollection _cardCollection;

        private bool _canClearCache = true;
        private bool _canDeleteCollection = true;

        public ICommand ClearCacheCommand { get; set; }
        public ICommand DeleteCollectionCommand { get; set; }

        private List<SettingGroup> _settingsGroups
            = new List<SettingGroup>();
        
        public List<SettingGroup> SettingsGroups
        {
            get => _settingsGroups;
            set
            {
                _settingsGroups = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(INavigationService navigationService,
            IApi api,
            IAlertService alertService,
            ICardCollection cardCollection)
            : base(navigationService)
        {
            Title = "Settings";
            _api = api;
            _alertService = alertService;
            _cardCollection = cardCollection;
        }

        protected override void SetUpCommands()
        {
            ClearCacheCommand = new Command
            (
                async () => await ClearCacheAsync(),
                () => _canClearCache
            );
            DeleteCollectionCommand = new Command
            (
                async () => await DeleteCollectionAsync(),
                () => _canDeleteCollection
            );
        }

        protected override Task OnLoadAsync()
        {
            var dataGroup = new SettingGroup("Data")
            {
                new ActionSetting
                (
                    "Clear Cache",
                    ClearCacheCommand,
                    "This option Clears the Cached API data. This will not clear your collection data from your device.",
                    isDestructive: true
                ),

                new ActionSetting
                (
                    "Delete Collection",
                    DeleteCollectionCommand,
                    "This option removes all set(s) and cards associated with those sets locally. This is not reverseable.",
                    isDestructive: true
                )
            };

            SettingsGroups.Add(dataGroup);

            return base.OnLoadAsync();
        }

        private async Task ClearCacheAsync()
        {
            if (!_canClearCache)
            {
                return;
            }

            _canClearCache = false;

            var shouldClearCache = await _alertService.ShowAlertAsync
            (
                "Clear Cached Data", 
                "Are you sure you want to clear the cached API data?", 
                "Clear Cache", 
                "Keep Cached Data"
            );

            if (!shouldClearCache)
            {
                _canClearCache = true;
                return;
            }

             await _api.ClearCacheAsync();
            _canClearCache = true;
        }

        private async Task DeleteCollectionAsync()
        {
            if (!_canDeleteCollection)
            {
                return;
            }

            _canDeleteCollection = false;


            var shouldDeleteCollection = await _alertService.ShowAlertAsync
            (
                "Delete Collection Data",
                "Are you sure you want to clear your collection data (sets and cards)?",
                "Delete Data",
                "Keep My Collection"
            );

            if (!shouldDeleteCollection)
            {
                _canDeleteCollection = true;
                return;
            }

            await _cardCollection.DeleteAllDataAsync();
            _canDeleteCollection = true;
        }

    }

    public class SettingGroup : List<BaseSetting>
    {
        public string Title { get; private set; }

        public SettingGroup(string title)
        {
            Title = title;
        }
    }
}
