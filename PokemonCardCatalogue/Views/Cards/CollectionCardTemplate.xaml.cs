using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PokemonCardCatalogue.Views.Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionCardTemplate : DataTemplate
    {
        public CollectionCardTemplate()
        {
            InitializeComponent();
        }
    }
}