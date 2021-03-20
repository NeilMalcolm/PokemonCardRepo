using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Pages;
using PokemonCardCatalogue.Services.Interfaces;
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

        private string _setId;

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

        public ICommand GoToCardCommand { get; set; }

        public SetListViewModel(ISetListLogic setListLogic,
            INavigationService navigationService)
            : base(navigationService)
        {
            _setListLogic = setListLogic;
            ReloadDataOnAppearing = true;
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
            GoToCardCommand = new Command<Card>(async (card) => await GoToCardAsync(card), (card) => _canGoToCard);
        }

        protected override async Task OnLoadAsync()
        {
            CardList = await _setListLogic.GetAllCardsForSetAsync(_setId);
        }

        private async Task GoToCardAsync(Card card)
        {
            _canGoToCard = false;
            await NavigationService.GoToAsync<CardPage>(card);
            _canGoToCard = true;
        }
    }
}
