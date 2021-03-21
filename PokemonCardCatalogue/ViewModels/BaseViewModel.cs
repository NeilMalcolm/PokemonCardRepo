using PokemonCardCatalogue.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PokemonCardCatalogue.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public bool ReloadDataOnAppearing { get; protected set; } = false;

        protected INavigationService NavigationService;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _title;

        public string Title
        {
            get => _title;
            set 
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading; 
            set 
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; set; }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            SetUpCommands();
        }

        public virtual void Init(object parameter)
        {
        }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                await OnLoadAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual Task OnLoadAsync()
        {
            Title = "Pokétracker";
            return Task.CompletedTask;
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetUpCommands()
        {

        }
    }
}
