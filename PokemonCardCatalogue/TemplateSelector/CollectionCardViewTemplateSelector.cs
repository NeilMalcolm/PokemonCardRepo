using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Views.Cards;
using Xamarin.Forms;

namespace PokemonCardCatalogue.TemplateSelector
{
    public class CollectionCardViewTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate OwnedCardTemplate = new CollectionCardTemplate();
        private readonly DataTemplate UnownedCardTemplate = new UnownedCollectionCardTemplate();

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
