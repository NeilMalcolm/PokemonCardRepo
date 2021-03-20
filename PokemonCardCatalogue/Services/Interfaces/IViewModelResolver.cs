using PokemonCardCatalogue.ViewModels;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface IViewModelResolver
    {
        BaseViewModel Get<T>() where T : Page;
        void Register<TPage, TViewModel>()  where TPage : Page
                                            where TViewModel : BaseViewModel;
    }
}
