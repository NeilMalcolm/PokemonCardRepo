namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface IDependencyHandler
    {
        void Init();
        void RegisterEarlyPlatformSpecificDependencies();
        void RegisterLatePlatformSpecificDependencies();
        T Get<T>() where T : class;
    }
}
