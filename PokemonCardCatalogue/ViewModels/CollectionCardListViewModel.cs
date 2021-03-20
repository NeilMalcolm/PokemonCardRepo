using PokemonCardCatalogue.Logic.Interfaces;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Services.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public enum SortOrder
    {
        Unspecified,
        NumericAscending,
        NumericDescending,
        Rarity
    }

    public class CollectionCardListViewModel : BaseViewModel
    {
        private readonly ICollectionLogic _collectionLogic;

        private int _offset = 0;
        private int _limit = 39;

        private bool _canNavigateToCard = true;
        private string _setId;
        private int _allCardsCount;
        private IEnumerable<CardItem> _allCardItems;

        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private List<KeyValuePair<SortOrder, string>> _sortModes;
        public List<KeyValuePair<SortOrder, string>> SortModes
        {
            get
            {
                return _sortModes ??= new List<KeyValuePair<SortOrder, string>>
                {
                    new KeyValuePair<SortOrder, string>(SortOrder.NumericAscending, "Number Asc"),
                    new KeyValuePair<SortOrder, string>(SortOrder.NumericDescending, "Number Desc"),
                    new KeyValuePair<SortOrder, string>(SortOrder.Rarity, "Rarity"),
                };
            }
        }

        private ObservableList<CardItem> _cardItemList;
        public ObservableList<CardItem> CardItemList
        {
            get => _cardItemList;
            set
            {
                _cardItemList = value;
                OnPropertyChanged();
            }
        }

        private KeyValuePair<SortOrder, string> _currentSortOrder;
        public KeyValuePair<SortOrder, string> CurrentSortOrder
        {
            get => _currentSortOrder;
            set
            {
                if (_currentSortOrder.Key != value.Key)
                {
                    HandleSortOrderChange(value.Key);
                }
                _currentSortOrder = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToCardCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand LoadMoreCardItemsCommand { get; set; }

        public CollectionCardListViewModel
            (
                INavigationService navigationService,
                ICollectionLogic collectionLogic
            ) 
            : base(navigationService)
        {
            _collectionLogic = collectionLogic;
        }

        public override void Init(object parameter)
        {
            if (parameter is string setId)
            {
                _setId = setId;
            }
        }

        protected override void SetUpCommands()
        {
            GoToCardCommand = new Command<CardItem>
            (
                async (cardItem) => await GoToCardAsync(cardItem),
                (cardItem) => _canNavigateToCard
            );
            AddCommand = new Command<CardItem>(async (cardItem) => await AddAsync(cardItem));
            LoadMoreCardItemsCommand = new Command(LoadMore);
        }

        protected override async Task OnLoadAsync()
        {
            _allCardItems = await _collectionLogic.GetCardsForSetAsync(_setId);
            _allCardsCount = _allCardItems.Count();

            CurrentSortOrder = SortModes.FirstOrDefault();
        }

        private void SetCardItemList()
        {
            ResetOffset();
            CardItemList?.Clear();
            CardItemList = new ObservableList<CardItem>(_allCardItems
                .Take(_limit));

            _offset += _limit;
        }

        private void LoadMore()
        {
            var currentCount = CardItemList.Count;
            var maxCount = _allCardsCount;

            if (currentCount == maxCount)
            {
                return;
            }

            var limit = _limit;
            if (currentCount + limit > maxCount)
            {
                limit = maxCount - currentCount;
            }

            CardItemList.AddRange
            (
                _allCardItems.Skip(_offset)
                    .Take(limit)
            );

            _offset += limit;
            OnPropertyChanged(nameof(CardItemList));
        }

        private async Task GoToCardAsync(CardItem cardItem)
        {
            _canNavigateToCard = false;
            //await NavigationService.GoToAsync<>();
            _canNavigateToCard = true;
        }

        private async Task AddAsync(CardItem item)
        {
            item.IncrementOwnedCount();

            await semaphore.WaitAsync();
            try
            {
                _ = await _collectionLogic.SetOwnedCountForCard(item);
            }
            catch
            {

            }
            finally
            {
                semaphore.Release();
            }
        }

        private void HandleSortOrderChange(SortOrder newSortOrder)
        {
            var currentSortOrder = CurrentSortOrder.Key;
            if (newSortOrder == currentSortOrder)
            {
                return;
            }

            switch (newSortOrder)
            {
                case SortOrder.Rarity:
                    _allCardItems = _allCardItems.OrderBy(x => x.Card.Rarity)
                        .ThenBy(x => x.CacheId);
                    break;
                case SortOrder.NumericDescending:
                    _allCardItems = _allCardItems.OrderByDescending(x => x.CacheId);
                    break;
                default:
                case SortOrder.NumericAscending:
                    _allCardItems = _allCardItems.OrderBy(x => x.CacheId);
                    break;
            }

            SetCardItemList();
        }

        private void ResetOffset()
        {
            _offset = 0;
        }
    }

    public class ObservableList<T> : IList<T>, INotifyCollectionChanged
    {
        private List<T> _internalList = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableList(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public T this[int index] { get => _internalList[index]; set => _internalList[index] = value; }

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _internalList.Add(item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _internalList.Clear();
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(T item)
        {
            var result = _internalList.Remove(item);
            if(result)
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }

            return result;
        }

        public void RemoveAt(int index)
        {
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _internalList[index]));
            _internalList.RemoveAt(index);
        }

        public void AddRange(IEnumerable<T> items)
        {
            _internalList.AddRange(items);

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}
