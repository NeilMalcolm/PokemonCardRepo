using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Services.Interfaces;

namespace PokemonCardCatalogue.ViewModels
{
    public class CardViewModel : BaseViewModel
    {
        private readonly ICardLogic _cardLogic;

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

        public CardViewModel(ICardLogic cardLogic,
            INavigationService navigationService) 
            : base(navigationService)
        {
            _cardLogic = cardLogic;
        }

        public override void Init(object parameter)
        {
            if (parameter is Card card)
            {
                ThisCard = card;
            }
        }
    }
}
