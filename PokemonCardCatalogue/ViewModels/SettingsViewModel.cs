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

        private bool _canClearCache = true;

        public ICommand ClearCacheCommand { get; set; }

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
            IAlertService alertService)
            : base(navigationService)
        {
            _api = api;
            _alertService = alertService;
        }

        protected override void SetUpCommands()
        {
            ClearCacheCommand = new Command
            (
                async () => await ClearCacheAsync(),
                () => _canClearCache
            );
        }

        protected override Task OnLoadAsync()
        {
            var dataGroup = new SettingGroup("Data")
            {
                new ActionSetting("Clear Cache", ClearCacheCommand)
            };

            SettingsGroups.Add(dataGroup);

            return base.OnLoadAsync();
        }

        private async Task ClearCacheAsync()
        {
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
