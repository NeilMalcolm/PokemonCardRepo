using PokemonCardCatalogue.Models.Settings;

namespace PokemonCardCatalogue.Models.Settings
{
    public abstract class BaseSetting
    {
        public string Name { get; private set; }
        public SettingType Type { get; private set; }

        public BaseSetting(string name, SettingType type)
        {
            Name = name;
            Type = type;
        }
    }
}
