using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CollectionSetsViewModel : BaseViewModel
    {
        private readonly ICollectionLogic _collectionLogic;
        private readonly IAlertService _alertService;

        private bool _canGoToSet = true;

        private ObservableCollection<SetItem> _setItems;
        public ObservableCollection<SetItem> SetItems
        {
            get => _setItems;
            set
            {
                _setItems = value;
                OnPropertyChanged();
            }
        }

        List<SetItem> _deletingSetItems { get; set; } = new List<SetItem>();

        public ICommand DeleteSetCommand { get; set; }
        public ICommand GoToSetCommand { get; set; } 

        public CollectionSetsViewModel(INavigationService navigationService,
            ICollectionLogic collectionLogic,
            IAlertService alertService) 
            : base(navigationService)
        {
            _collectionLogic = collectionLogic;
            _alertService = alertService;
        }

        protected override void SetUpCommands()
        {
            RefreshCommand = new Command(async () => await OnLoadAsync(), () => !IsLoading);
            DeleteSetCommand = new Command<SetItem>(async (setItem) => await ConfirmDeleteSet(setItem), (setItem) => CanDeleteSet(setItem));
            GoToSetCommand = new Command<SetItem>(async (setItem) => await GoToSetAsync(setItem), (setItem) => _canGoToSet);
        }

        protected override async Task OnLoadAsync()
        {
            SetItems = new ObservableCollection<SetItem>(await _collectionLogic.GetAllSets(withCount: true));
        }

        private async Task ConfirmDeleteSet(SetItem setItem)
        {
            var shouldDelete = await _alertService.ShowAlertAsync
            (
                $"Confirm Delete Set", 
                "You are about to delete {setItem.Set.Name} from your collection. This will remove all cards too. Do you want to delete?",
                "Delete", 
                "Do not delete"
            );

            if (!shouldDelete)
            {
                return;
            }

            await DeleteSet(setItem);
        }

        private async Task GoToSetAsync(SetItem item)
        {
            _canGoToSet = false;
            await NavigationService.GoToAsync<CollectionCardListPage>(item.Set.Id);
            _canGoToSet = true;
        }

        private async Task DeleteSet(SetItem setItem)
        {
            _deletingSetItems.Add(setItem);
            await _collectionLogic.DeleteSetAsync(setItem.Set);
            _deletingSetItems.Remove(setItem);
            SetItems.Remove(setItem);
        }


        private bool CanDeleteSet(SetItem setItem)
        {
            return !_deletingSetItems.Any(s => s.Set.Id == setItem.Set.Id);
        }
    }
}
