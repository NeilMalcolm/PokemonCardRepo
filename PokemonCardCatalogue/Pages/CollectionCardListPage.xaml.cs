using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PokemonCardCatalogue.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionCardListPage : BaseContentPage
    {
        public CollectionCardListPage()
        {
            InitializeComponent();
        }

        private void CollectionView_BindingContextChanged(object sender, System.EventArgs e)
        {
            if (sender is CollectionView collectionView)
            {
                collectionView.ScrollTo(0, animate: false);
            }
        }
    }
}