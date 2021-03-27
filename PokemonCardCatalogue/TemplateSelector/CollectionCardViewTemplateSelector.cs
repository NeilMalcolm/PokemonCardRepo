using PokemonCardCatalogue.Models;
using PokemonCardCatalogue.Views.Cards;
using Xamarin.Forms;

namespace PokemonCardCatalogue.TemplateSelector
{
    public class CollectionCardViewTemplateSelector : DataTemplateSelector
    {
        private DataTemplate OwnedCardTemplate = new CollectionCardTemplate();
        private DataTemplate UnownedCardTemplate = new UnownedCollectionCardTemplate();

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is CardItem cardItem)
            {
                if (cardItem.Owned)
                {
                    return OwnedCardTemplate;
                }
                else
                {
                    return UnownedCardTemplate;
                }
            }

            return null;
        }
    }
}
