using PokemonCardCatalogue.Models.Settings;
using PokemonCardCatalogue.Views.Settings;
using Xamarin.Forms;

namespace PokemonCardCatalogue.TemplateSelector
{
    public class SettingTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ActionSetting)
            {
                return new ActionSettingDataTemplate();
            }

            return null;
        }
    }
}
