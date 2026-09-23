// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace CMiX.Studio.Avalonia.Views
{
    public class ViewModelToViewTemplate : IDataTemplate
    {
        private readonly Assembly _viewsAssembly;
        private readonly Dictionary<Type, Type?> _cache = new();

        public ViewModelToViewTemplate()
        {
            _viewsAssembly = typeof(ViewsAssemblyMarker).Assembly;
        }

        public bool Match(object? data)
        {
            return data != null && ResolveViewType(data.GetType()) != null;
        }

        public Control Build(object? data)
        {
            var viewType = ResolveViewType(data!.GetType());
            if (viewType == null)
                throw new InvalidOperationException($"No view found for view model type '{data.GetType().Name}'.");

            return (Control)Activator.CreateInstance(viewType)!;
        }

        private Type? ResolveViewType(Type vmType)
        {
            if (_cache.TryGetValue(vmType, out var cached))
                return cached;

            string viewNamespace = "CMiX.Studio.Avalonia.Views";
            var viewType = _viewsAssembly.GetType($"{viewNamespace}.{vmType.Name}");

            _cache[vmType] = viewType!;
            return viewType;
        }
    }
}
