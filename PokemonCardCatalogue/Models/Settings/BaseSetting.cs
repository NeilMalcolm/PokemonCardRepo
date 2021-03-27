using PokemonCardCatalogue.Models.Settings;

namespace PokemonCardCatalogue.Models.Settings
{
    public abstract class BaseSetting
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public SettingType Type { get; private set; }

        public BaseSetting(string name, SettingType type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }
    }
}
