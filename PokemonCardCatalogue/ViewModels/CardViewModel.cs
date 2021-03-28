using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Helpers.Factories;
using PokemonCardCatalogue.Logic.Interfaces;
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
        private readonly SemaphoreSlim ownedCountSemaphore = new SemaphoreSlim(1, 1);

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

        private int _ownedCount;
        public int OwnedCount
        {
            get => _ownedCount; 
            set 
            {
                _ownedCount = value;
                OnPropertyChanged();
                DecrementOwnedCountCommand.ChangeCanExecute();
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

        public bool IsRelatedCardsSectionVisible => RelatedCards?.Count > 0;

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

        public bool IsPricesSectionVisible => Prices?.Count > 0;

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

        public ICommand GoToRelatedCardCommand { get; set; }
        public Command DecrementOwnedCountCommand { get; set; }
        public ICommand IncrementOwnedCountCommand { get; set; }

        public CardViewModel(INavigationService navigationService,
            ICardLogic cardLogic,
            ICollectionLogic collectionLogic,
            IVibrationService vibrationService) 
            : base(navigationService)
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

            DecrementOwnedCountCommand = new Command
            (
                async () => await DecrementOwnedCount(),
                () => OwnedCount > 0
            );

            IncrementOwnedCountCommand = new Command
            (
                async () => await IncrementOwnedCount()
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

        private async Task DecrementOwnedCount()
        {
            try
            {
                if (OwnedCount == 0)
                {
                    return;
                }

                OwnedCount--;
                await ownedCountSemaphore.WaitAsync();
                _vibrationService.PerformSelectionFeedbackVibration();
                await _collectionLogic.DecrementCardOwnedCount(ThisCard.Id);
            }
            finally
            {
                if (ownedCountSemaphore.CurrentCount == 0)
                {
                    ownedCountSemaphore.Release();
                }
            }
        }

        private async Task IncrementOwnedCount()
        {
            try
            {
                OwnedCount++;
                await ownedCountSemaphore.WaitAsync();
                _vibrationService.PerformSelectionFeedbackVibration();
                await _collectionLogic.IncrementCardOwnedCount(ThisCard.Id);
            }
            finally
            {
                if (ownedCountSemaphore.CurrentCount == 0)
                {
                    ownedCountSemaphore.Release();
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
            OwnedCount = await _collectionLogic.GetCardOwnedCount(id);
        }

        private async Task SetPrices(Card card)
        {
            try
            {
                var cards = await GetPricesAsync(card);
                await Task.Delay(800);
                Prices = cards;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoadingPrices = false;
            }
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
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoadingRelatedCards = false;
            }
        }
    }
}
