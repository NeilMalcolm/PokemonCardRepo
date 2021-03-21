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

        private SetItem _previouslyNavigatedToSetItem;

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
            ReloadDataOnAppearing = true;
        }

        protected override void SetUpCommands()
        {
            RefreshCommand = new Command(async () => await OnLoadAsync(), () => !IsLoading);
            DeleteSetCommand = new Command<SetItem>(async (setItem) => await ConfirmDeleteSet(setItem), (setItem) => CanDeleteSet(setItem));
            GoToSetCommand = new Command<SetItem>(async (setItem) => await GoToSetAsync(setItem), (setItem) => _canGoToSet);
        }

        protected override async Task OnLoadAsync()
        {
            var allSets = await _collectionLogic.GetAllSets(withCount: true);

            if (allSets.Count != SetItems?.Count)
            {
                SetItems = new ObservableCollection<SetItem>(allSets);
            }
            else if (DoPreviouslyNavigatedSetAndSetFromDbHaveDifferentOwnedCounts(allSets, _previouslyNavigatedToSetItem))
            {
                var item = allSets?.FirstOrDefault(x => x.Set.Id == _previouslyNavigatedToSetItem.Set.Id);
                var index = SetItems.IndexOf(item);
                SetItems.RemoveAt(index);
                SetItems.Insert(index, item);
            }

            _previouslyNavigatedToSetItem = null;
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
            _previouslyNavigatedToSetItem = item;
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

        private bool DoPreviouslyNavigatedSetAndSetFromDbHaveDifferentOwnedCounts(List<SetItem> setItems, SetItem previouslyNavigated)
        {
            if (setItems?.Count == 0 || previouslyNavigated is null)
            {
                return true;
            }

            var matchingItem = setItems?.FirstOrDefault(x => x.Set.Id == previouslyNavigated.Set.Id);
            if (matchingItem is null)
            {
                return true;
            }

            if (previouslyNavigated.OwnedCount != matchingItem.OwnedCount)
            {
                return true;
            }

            return false;
        }

    }
}
