using PokemonCardCatalogue.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms;
namespace PokemonCardCatalogue
{
    public partial class AppShell : Shell
    {
        public static Dictionary<string, ShellContent> Tabs;

        public AppShell()
        {
            InitializeComponent();
            SetNavBarHasShadow(this, false);
            Tabs = new Dictionary<string, ShellContent>
            {
                { HomeTab.Route, HomeTab },
                { CollectionTab.Route, CollectionTab },
                { SettingsTab.Route, SettingsTab }
            };
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
