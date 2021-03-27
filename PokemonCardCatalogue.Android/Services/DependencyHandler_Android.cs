using PokemonCardCatalogue.Services;
using PokemonCardCatalogue.Services.Interfaces;

namespace PokemonCardCatalogue.Droid.Services
{
    public class DependencyHandler_Android : BaseDependencyHandler
    {
        public DependencyHandler_Android(IDependencyContainer dependencyContainer) 
            : base(dependencyContainer)
        {
        }

        public override void RegisterEarlyPlatformSpecificDependencies()
        {
            _dependencyContainer.Register<IVibrationService, VibrationService_Android>(true);
        }

        public override void RegisterLatePlatformSpecificDependencies()
        {
        }
    }
}