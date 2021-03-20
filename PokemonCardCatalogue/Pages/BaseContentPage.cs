using PokemonCardCatalogue.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Pages
{
    public class BaseContentPage : ContentPage
    {
        private bool hasLoadedData = false;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is BaseViewModel baseViewModel)
            {
                if (baseViewModel.ReloadDataOnAppearing || !hasLoadedData)
                {
                    Task.Run(async () => 
                    { 
                        await baseViewModel.LoadAsync();
                        hasLoadedData = true;
                    });
                }
            }
        }
    }
}
