// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace CMiX.Studio.Views.Modifiers
{
    //public static class ModifierDataTemplateGenerator
    //{
    //    /// <summary>
    //    /// Generates DataTemplates and adds them to the provided ResourceDictionary.
    //    /// </summary>
    //    /// <param name="resources">ResourceDictionary to add the templates to (optional, defaults to Application.Current.Resources)</param>
    //    /// <param name="types">List of types to generate templates for</param>
    //    public static void GenerateTemplates(ResourceDictionary resources, IEnumerable<Type> types)
    //    {
    //        if (resources == null)
    //            resources = Application.Current.Resources;

    //        if (types == null) return;

    //        foreach (var type in types)
    //        {
    //            // Build the view type name using convention: TypeName -> CMiX.Studio.Views.TypeName
    //            var viewTypeName = $"CMiX.Studio.Views.{type.Name}";
    //            var viewType = Type.GetType(viewTypeName);

    //            if (viewType == null) continue;

    //            // Create the DataTemplate
    //            var factory = new FrameworkElementFactory(viewType);
    //            factory.SetBinding(FrameworkElement.DataContextProperty, new Binding());

    //            var template = new DataTemplate
    //            {
    //                DataType = type,
    //                VisualTree = factory
    //            };

    //            // Add template to the ResourceDictionary
    //            resources[type] = template;
    //        }
    //    }
    //}
}
