using System.Windows.Input;

namespace PokemonCardCatalogue.Models.Settings
{
    public class ActionSetting : BaseSetting
    {
        public ICommand Command { get; private set; }
        public bool IsDestructive { get; private set; }

        public ActionSetting
        (
            string name, 
            ICommand command, 
            string description,
            bool isDestructive = false
        )
           : base (name, SettingType.Action, description)
        {
            Command = command;
        }
    }
}
