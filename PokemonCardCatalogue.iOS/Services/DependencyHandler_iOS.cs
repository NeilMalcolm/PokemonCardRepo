using PokemonCardCatalogue.Services;
using PokemonCardCatalogue.Services.Interfaces;

namespace PokemonCardCatalogue.iOS.Services
{
    public class DependencyHandler_iOS : BaseDependencyHandler
    {
        public DependencyHandler_iOS(IDependencyContainer dependencyContainer) 
            : base(dependencyContainer)
        {
        }

        public override void RegisterEarlyPlatformSpecificDependencies()
        {
        }

        public override void RegisterLatePlatformSpecificDependencies()
        {
        }
    }
}