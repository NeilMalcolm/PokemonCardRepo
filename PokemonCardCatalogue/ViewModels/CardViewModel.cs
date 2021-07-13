using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Helpers.Factories;
using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private readonly ICardLogic _cardLogic;
        private readonly ICollectionLogic _collectionLogic;
        private readonly IVibrationService _vibrationService;
        private readonly SemaphoreSlim _normalOwnedCountSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _holoOwnedCountSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _reverseOwnedCountSemaphore = new SemaphoreSlim(1, 1);

        private bool _canNavigatingToRelatedCard = true;

        private Card _thisCard;
        public Card ThisCard
        {
            get => _thisCard;
            set
            {
                _thisCard = value;
                OnPropertyChanged();
            }
        }

        private int _normalOwnedCount;
        public int NormalOwnedCount
        {
            get => _normalOwnedCount;
            set
            {
                _normalOwnedCount = value;
                OnPropertyChanged();
                DecrementNormalOwnedCountCommand.ChangeCanExecute();
            }
        }
        
        private int _holoOwnedCount;
        public int HoloOwnedCount
        {
            get => _holoOwnedCount;
            set
            {
                _holoOwnedCount = value;
                OnPropertyChanged();
                DecrementHoloOwnedCountCommand.ChangeCanExecute();
            }
        }
        
        private int _reverseOwnedCount;
        public int ReverseOwnedCount
        {
            get => _reverseOwnedCount;
            set
            {
                _reverseOwnedCount = value;
                OnPropertyChanged();
                DecrementReverseOwnedCountCommand.ChangeCanExecute();
            }
        }
        
        private bool _showNormalCounter;
        public bool ShowNormalCounter
        {
            get => _showNormalCounter;
            set
            {
                _showNormalCounter = value;
                OnPropertyChanged();
            }
        }

        private bool _showReverseCounter;
        public bool ShowReverseCounter
        {
            get => _showReverseCounter;
            set
            {
                _showReverseCounter = value;
                OnPropertyChanged();
            }
        }
        
        private bool _showHoloCounter;
        public bool ShowHoloCounter
        {
            get => _showHoloCounter;
            set
            {
                _showHoloCounter = value;
                OnPropertyChanged();
            }
        }

        private List<Card> _relatedCards;
        public List<Card> RelatedCards
        {
            get => _relatedCards;
            set
            {
                _relatedCards = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRelatedCardsSectionVisible));
            }
        }

        public bool IsRelatedCardsSectionVisible => _relatedCards?.Count > 0;

        private List<PriceDisplayViewModel> _prices;
        public List<PriceDisplayViewModel> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPricesSectionVisible));
            }
        }
        
        private PriceDisplayViewModel _currentlyDisplayingPrice;
        public PriceDisplayViewModel CurrentlyDisplayingPrice
        {
            get => _currentlyDisplayingPrice;
            set
            {
                _currentlyDisplayingPrice = value;
                OnPropertyChanged();
            }
        }

        public bool IsPricesSectionVisible => _prices?.Count > 0;

        private bool _isLoadingRelatedCards;

        public bool IsLoadingRelatedCards
        {
            get => _isLoadingRelatedCards;
            set
            {
                _isLoadingRelatedCards = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isLoadingPrices;

        public bool IsLoadingPrices
        {
            get => _isLoadingPrices;
            set
            {
                _isLoadingPrices = value;
                OnPropertyChanged();
            }
        }

        public bool HasOtherRarities
        {
            get => Prices?.Count > 1;
        }

        public ICommand GoToRelatedCardCommand { get; set; }
        public Command DecrementNormalOwnedCountCommand { get; set; }
        public Command DecrementHoloOwnedCountCommand { get; set; }
        public Command DecrementReverseOwnedCountCommand { get; set; }
        public ICommand IncrementNormalOwnedCountCommand { get; set; }
        public ICommand IncrementHoloOwnedCountCommand { get; set; }
        public ICommand IncrementReverseOwnedCountCommand { get; set; }

        public CardViewModel(INavigationService navigationService,
            ICardLogic cardLogic,
            ICollectionLogic collectionLogic,
            IVibrationService vibrationService,
            ILog log
        ) : base(navigationService, log)
        {
            _cardLogic = cardLogic;
            _collectionLogic = collectionLogic;
            _vibrationService = vibrationService;
        }

        public override void Init(object parameter)
        {
            if (parameter is Card card)
            {
                ThisCard = card;
            }
        }

        protected override void SetUpCommands()
        {
            GoToRelatedCardCommand = new Command<Card>
            (
                async (card) => await GoToRelatedCard(card), 
                (card) => _canNavigatingToRelatedCard
            );

            DecrementNormalOwnedCountCommand = new Command
            (
                async () => await DecrementNormalOwnedCount(),
                () => NormalOwnedCount > 0
            );
            
            DecrementHoloOwnedCountCommand = new Command
            (
                async () => await DecrementHoloOwnedCount(),
                () => HoloOwnedCount > 0
            );
            
            DecrementReverseOwnedCountCommand = new Command
            (
                async () => await DecrementReverseOwnedCount(),
                () => ReverseOwnedCount > 0
            );

            IncrementNormalOwnedCountCommand = new Command
            (
                async () => await IncrementNormalOwnedCount()
            );
            IncrementHoloOwnedCountCommand = new Command
            (
                async () => await IncrementHoloOwnedCount()
            );
            IncrementReverseOwnedCountCommand = new Command
            (
                async () => await IncrementReverseOwnedCount()
            );
        }

        protected override async Task OnLoadAsync()
        {
            IsLoadingRelatedCards = true;
            IsLoadingPrices = true;
            var loadRelatedCardsTask = LoadRelatedCardsFromSameSetAsync();
            var getOwnedCountTask = GetOwnedCountAsync(ThisCard.Id);
            var pricesTask = SetPrices(ThisCard);

            await Task.WhenAll(getOwnedCountTask, loadRelatedCardsTask, pricesTask);
        }

        private void SetPriceCounters()
        {
            ShowNormalCounter = ThisCard.TcgPlayer?.Prices?.Normal != null;
            ShowReverseCounter = ThisCard.TcgPlayer?.Prices?.ReverseHolofoil != null;
            ShowHoloCounter = ThisCard.TcgPlayer?.Prices?.Holofoil != null;
        }


        private async Task DecrementHoloOwnedCount()
        {
            HoloOwnedCount = await DecrementCount(HoloOwnedCount, _holoOwnedCountSemaphore, _collectionLogic.DecrementCardHoloOwnedCount(ThisCard.Id));
        }
        
        private async Task DecrementReverseOwnedCount()
        {
            ReverseOwnedCount = await DecrementCount(ReverseOwnedCount, _reverseOwnedCountSemaphore, _collectionLogic.DecrementCardReverseOwnedCount(ThisCard.Id));
        }
        
        private async Task DecrementNormalOwnedCount()
        {
            NormalOwnedCount = await DecrementCount(NormalOwnedCount, _normalOwnedCountSemaphore, _collectionLogic.DecrementCardNormalOwnedCount(ThisCard.Id));
        }

        private async Task<int> DecrementCount(int ownedCount, SemaphoreSlim semaphore, Task<int> onDecrement)
        {
            try
            {
                if (ownedCount == 0)
                {
                    return 0;
                }

                ownedCount--;
                await semaphore.WaitAsync();
                _vibrationService.PerformSelectionFeedbackVibration();
                await onDecrement;
                return ownedCount;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return 0;
            }
            finally
            {
                if (semaphore.CurrentCount == 0)
                {
                    semaphore.Release();
                }
            }
        }

        private async Task IncrementHoloOwnedCount()
        {
            HoloOwnedCount = await IncrementCount(HoloOwnedCount, _holoOwnedCountSemaphore, _collectionLogic.IncrementCardHoloOwnedCount(ThisCard.Id));
        }

        private async Task IncrementReverseOwnedCount()
        {
            ReverseOwnedCount = await IncrementCount(ReverseOwnedCount, _reverseOwnedCountSemaphore, _collectionLogic.IncrementCardReverseOwnedCount(ThisCard.Id));
        }

        private async Task IncrementNormalOwnedCount()
        {
            NormalOwnedCount = await IncrementCount(NormalOwnedCount, _normalOwnedCountSemaphore, _collectionLogic.IncrementCardNormalOwnedCount(ThisCard.Id));
        }

        private async Task<int> IncrementCount(int ownedCount, SemaphoreSlim semaphore, Task<int> onDecrement)
        {
            try
            {
                ownedCount++;
                await semaphore.WaitAsync();
                _vibrationService.PerformSelectionFeedbackVibration();
                await onDecrement;
                return ownedCount;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);

                return 0;
            }
            finally
            {
                if (semaphore.CurrentCount == 0)
                {
                    semaphore.Release();
                }
            }
        }
        
        private async Task GoToRelatedCard(Card card)
        {
            _canNavigatingToRelatedCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canNavigatingToRelatedCard = true;
        }

        private async Task GetOwnedCountAsync(string id)
        {
            var cardOwnedCounts = await _collectionLogic.GetCardOwnedCounts(id); 

            NormalOwnedCount = cardOwnedCounts.NormalCount;
            HoloOwnedCount = cardOwnedCounts.HoloCount;
            ReverseOwnedCount = cardOwnedCounts.ReverseCount;
        }

        private async Task SetPrices(Card card)
        {
            try
            {
                var prices = await GetPricesAsync(card);
                await Task.Delay(800);
                Prices = prices;
                SetCurrentlyDisplayingPrice(Prices);
                SetPriceCounters();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                IsLoadingPrices = false;
                OnPropertyChanged(nameof(HasOtherRarities));
            }
        }

        private void SetCurrentlyDisplayingPrice(IList<PriceDisplayViewModel> prices)
        {
            if (prices?.Count == 0)
            {
                return;
            }

            CurrentlyDisplayingPrice = prices[0];
        }

        private Task<List<PriceDisplayViewModel>> GetPricesAsync(Card card)
        {
            return Task.FromResult
            (
                PriceDisplayFactory.GetPriceDisplayViewModels(card.TcgPlayer?.Prices)
            );
        }

        private async Task LoadRelatedCardsFromSameSetAsync()
        {
            try
            {
                var results = await _cardLogic.GetRelatedCardsInSetAsync(_thisCard);
                await Task.Delay(300);
                RelatedCards = results;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                IsLoadingRelatedCards = false;
            }
        }
    }
}
