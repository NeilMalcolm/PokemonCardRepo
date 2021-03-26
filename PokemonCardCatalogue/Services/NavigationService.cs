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

        public Task SwitchTab(string tabRoute)
        {
            Shell.Current.CurrentItem.CurrentItem = AppShell.Tabs[tabRoute];
            return Task.CompletedTask;
        }

        private Page GetPageAndViewModel<T>(object parameter = null) where T : Page
        {
            var viewModel = _viewModelResolver.Get<T>();
            Page page;
            try
            {
                page = Activator.CreateInstance<T>();
                viewModel.Init(parameter);

                page.BindingContext = viewModel;
                return page;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw ex;
            }
        }
    }
}
