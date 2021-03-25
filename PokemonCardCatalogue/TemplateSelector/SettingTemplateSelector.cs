using PokemonCardCatalogue.Models.Settings;
using PokemonCardCatalogue.Views.Settings;
using Xamarin.Forms;

namespace PokemonCardCatalogue.TemplateSelector
{
    public class SettingTemplateSelector : DataTemplateSelector
    {
        private DataTemplate ActionSettingTemplate = new ActionSettingDataTemplate();

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ActionSetting)
            {
                return ActionSettingTemplate;
            }

            return null;
        }
    }
}
