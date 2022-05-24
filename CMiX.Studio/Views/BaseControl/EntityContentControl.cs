using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views.BaseControl
{
    public class EntityContentControl : ContentControl
    {

        public EntityContentControl()
        {
            this.ContentTemplateSelector = new MyDataTemplateSelector();
        }


        class MyDataTemplateSelector : DataTemplateSelector
        {

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                var declaredDataTemplate = FindDeclaredDataTemplate(item, container);
                var wrappedDataTemplate = WrapDataTemplate(declaredDataTemplate);
                return wrappedDataTemplate;
            }

            private static DataTemplate WrapDataTemplate(DataTemplate declaredDataTemplate)
            {
                var frameworkElementFactory = new FrameworkElementFactory(typeof(ContentPresenter));
                frameworkElementFactory.SetValue(ContentPresenter.ContentTemplateProperty, declaredDataTemplate);
                var dataTemplate = new DataTemplate();
                dataTemplate.VisualTree = frameworkElementFactory;
                return dataTemplate;
            }

            private static DataTemplate FindDeclaredDataTemplate(object item, DependencyObject container)
            {
                var dataTemplateKey = new DataTemplateKey(item.GetType());
                var dataTemplate = ((FrameworkElement)container).FindResource(dataTemplateKey) as DataTemplate;
                if (dataTemplate == null)
                    throw new Exception("datatemplate not found");
                return dataTemplate;
            }
        }
    }
}
