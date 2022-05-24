using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public class ColorModifierTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RandomHSVTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if (item != null)
            {
                if (item is Core.Presentation.ViewModels.RandomHSV)
                    dataTemplate = RandomHSVTemplate;
            }

            return dataTemplate;
        }
    }
}
