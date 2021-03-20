using System.Windows.Input;

namespace PokemonCardCatalogue.Models.Settings
{
    public class ActionSetting : BaseSetting
    {
        public ICommand Command { get; set; }

        public ActionSetting(string name, ICommand command)
           : base (name, SettingType.Action)
        {
            Command = command;
        }
    }
}
