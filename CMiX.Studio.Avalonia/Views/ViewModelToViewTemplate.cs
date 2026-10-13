// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace CMiX.Studio.Avalonia.Views
{
    // Finds the view of a model by name: the view has the type name of the model. The views are
    // the controls in the root namespace and in its child namespaces, so a view can sit in any
    // folder. The child namespaces in ExcludedNamespaces hold support controls, not views.
    public class ViewModelToViewTemplate : IDataTemplate
    {
        private static readonly string[] ExcludedNamespaces = { "Controls", "Managers", "Menus", "Modifiers", "Modulation", "Windows" };

        private readonly Assembly _viewsAssembly;
        private readonly string _rootNamespace;
        private readonly string[] _excludedNamespaces;
        private readonly Dictionary<Type, Type?> _cache = new();
        private Dictionary<string, Type>? _index;

        public ViewModelToViewTemplate()
            : this(typeof(ViewsAssemblyMarker).Assembly, typeof(ViewsAssemblyMarker).Namespace!, ExcludedNamespaces)
        {
        }

        public ViewModelToViewTemplate(Assembly viewsAssembly, string rootNamespace, params string[] excludedNamespaces)
        {
            _viewsAssembly = viewsAssembly;
            _rootNamespace = rootNamespace;
            _excludedNamespaces = excludedNamespaces;
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

            _index ??= BuildIndex();
            _index.TryGetValue(vmType.Name, out var viewType);
            _cache[vmType] = viewType;
            return viewType;
        }

        private Dictionary<string, Type> BuildIndex()
        {
            var index = new Dictionary<string, Type>();
            foreach (var type in _viewsAssembly.GetTypes().Where(IsView))
            {
                if (index.TryGetValue(type.Name, out var other))
                    throw new InvalidOperationException(
                        $"Two views are named '{type.Name}': {other.FullName} and {type.FullName}.");

                index[type.Name] = type;
            }

            return index;
        }

        private bool IsView(Type type)
        {
            if (!type.IsClass || type.IsAbstract || !typeof(Control).IsAssignableFrom(type))
                return false;

            var ns = type.Namespace;
            if (ns == _rootNamespace)
                return true;

            if (ns == null || !ns.StartsWith(_rootNamespace + "."))
                return false;

            var childNamespace = ns.Substring(_rootNamespace.Length + 1);
            return !_excludedNamespaces.Any(excluded =>
                childNamespace == excluded || childNamespace.StartsWith(excluded + "."));
        }
    }
}
