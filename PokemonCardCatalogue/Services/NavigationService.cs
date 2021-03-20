using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using FormsApplication = Xamarin.Forms.Application;
using IFormsNavigation = Xamarin.Forms.INavigation;

namespace PokemonCardCatalogue.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IViewModelResolver _viewModelResolver;
        private IFormsNavigation CurrentNavigation => FormsApplication.Current.MainPage.Navigation;

        public NavigationService(IViewModelResolver viewModelResolver)
        {
            _viewModelResolver = viewModelResolver;
        }

        public Page GetMainPage()
        {
            return GetPageAndViewModel<AppShell>();
        }

        public Task GoToAsync<T>(object parameter = null) where T : Page
        {
            return CurrentNavigation.PushAsync(GetPageAndViewModel<T>(parameter));
        }

        public Task PopAsync()
        {
            return PopAsync();
        }

        private Page GetPageAndViewModel<T>(object parameter = null) where T : Page
        {
            var viewModel = _viewModelResolver.Get<T>();
            var page = Activator.CreateInstance<T>();
            viewModel.Init(parameter);

            page.BindingContext = viewModel;
            return page;
        }
    }
}
