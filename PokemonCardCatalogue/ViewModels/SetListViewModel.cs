using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PokemonCardCatalogue.ViewModels
{
    public class SetListViewModel : BaseViewModel
    {
        private readonly ISetListLogic _setListLogic;

        private bool _canGoToCard = true;

        private Set _set;

        private List<Card> _cardList;
        public List<Card> CardList
        {
            get => _cardList; 
            set 
            { 
                _cardList = value;
                OnPropertyChanged();
            }
        }

        private string _emptyListMessage;
        public string EmptyListMessage
        {
            get => _emptyListMessage;
            set
            {
                _emptyListMessage = value;
                OnPropertyChanged();
            }
        }

        public string SetImageUrl { get; set; }

        public ICommand GoToCardCommand { get; set; }


        public SetListViewModel
        (
            ISetListLogic setListLogic,
            INavigationService navigationService,
            ILog log
        ) : base(navigationService, log)
        {
            _setListLogic = setListLogic;
        }

        public override void Init(object parameter)
        {
            EmptyListMessage = string.Empty;

            if (parameter is Set set)
            {
                _set = set;
                Title = _set.Name;
                SetImageUrl = set.Images.Logo;
            }
        }

        protected override void SetUpCommands()
        {
            GoToCardCommand = new Command<Card>
            (
                async (card) => await GoToCardAsync(card), 
                (card) => _canGoToCard
            );
        }

        protected override async Task OnLoadAsync()
        {
            try
            {
                var allCards = await _setListLogic.GetAllCardsForSetAsync(_set.Id);
                await Task.Delay(500);
                CardList = allCards;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);

                EmptyListMessage = ErrorMessages.SetList.CollectionViewWebRequestTimeoutMessage;
            }
        }

        private async Task GoToCardAsync(Card card)
        {
            _canGoToCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canGoToCard = true;
        }
    }
}
