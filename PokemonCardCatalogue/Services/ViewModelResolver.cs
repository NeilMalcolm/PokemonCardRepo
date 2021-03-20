using PokemonCardCatalogue.Services.Interfaces;
using PokemonCardCatalogue.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Services
{
    public class ViewModelResolver : IViewModelResolver
    {
        private const string ViewModelNotRegisteredForPageExceptionMessage =
            @"ViewModel not found for {0}. Be sure to register the Page/ViewModel pair first.";

        private readonly Dictionary<Type, Type> _pageViewModelDictionary;
        private readonly IDependencyContainer _dependencyContainer;

        public ViewModelResolver(IDependencyContainer dependencyContainer)
        {
            _dependencyContainer = dependencyContainer;
            _pageViewModelDictionary = new Dictionary<Type, Type>();
        }

        public void Register<TPage, TViewModel>() where TPage : Page
                                           where TViewModel : BaseViewModel
        {
            _pageViewModelDictionary.Add(typeof(TPage), typeof(TViewModel));
        }

        public BaseViewModel Get<T>() where T : Page
        {
            var pageType = typeof(T);

            if (!_pageViewModelDictionary.ContainsKey(pageType))
            {
                throw new InvalidOperationException
                (
                    string.Format(ViewModelNotRegisteredForPageExceptionMessage, pageType.Name)
                );
            }

            var viewModelType = _pageViewModelDictionary[pageType];
            return (BaseViewModel)_dependencyContainer.Get(viewModelType);
        }
    }
}
