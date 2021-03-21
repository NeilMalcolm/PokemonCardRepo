using PokemonCardCatalogue.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace PokemonCardCatalogue.Pages
{
    public class BaseContentPage : ContentPage
    {
        private bool hasLoadedData = false;

        public BaseContentPage()
        {
            On<iOS>().SetUseSafeArea(true);
        }

        protected override void OnAppearing()
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
