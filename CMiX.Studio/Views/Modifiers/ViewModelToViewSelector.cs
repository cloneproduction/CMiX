// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public class ViewModelToViewSelector : DataTemplateSelector
    {
        private readonly Assembly _viewsAssembly;

        public ViewModelToViewSelector()
        {
            _viewsAssembly = typeof(Kuwahara).Assembly; // pick any View type in your Views assembly
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            string viewNamespace = "CMiX.Studio.Views";
            string viewName = item.GetType().Name; // exact match
            var viewType = _viewsAssembly.GetType($"{viewNamespace}.{viewName}");

            if (viewType == null)
            {
                System.Diagnostics.Debug.WriteLine($"View not found for {item.GetType().Name}");
                return null;
            }

            var template = new DataTemplate(item.GetType())
            {
                VisualTree = new FrameworkElementFactory(viewType)
            };

            System.Diagnostics.Debug.WriteLine($"Template selected for {item.GetType().Name}");
            return template;
        }
    }
}
