using PokemonCardCatalogue.ViewModels;
using Xamarin.Forms;

namespace PokemonCardCatalogue
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is BaseViewModel baseViewModel)
            {
                await baseViewModel.LoadAsync();
            }
        }
    }
}
