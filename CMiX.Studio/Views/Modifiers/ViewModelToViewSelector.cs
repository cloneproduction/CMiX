// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CMiX.Studio.Views
{
    public class ViewModelToViewSelector : DataTemplateSelector
    {
        private readonly Assembly _viewsAssembly;
        private readonly Dictionary<Type, DataTemplate> _cache = new();

        public ViewModelToViewSelector()
        {
            _viewsAssembly = typeof(ViewsAssemblyMarker).Assembly;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            var vmType = item.GetType();
            if (_cache.TryGetValue(vmType, out var cached))
                return cached;

            string viewNamespace = "CMiX.Studio.Views";
            var viewType = _viewsAssembly.GetType($"{viewNamespace}.{vmType.Name}");

            if (viewType == null) return null;

            var template = new DataTemplate(vmType)
            {
                VisualTree = new FrameworkElementFactory(viewType)
            };

            _cache[vmType] = template;
            return template;
        }
    }
}
