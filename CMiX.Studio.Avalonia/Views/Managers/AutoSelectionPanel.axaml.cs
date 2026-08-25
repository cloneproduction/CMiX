// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CMiX.Core.Prefabs;
using AvaloniaGrid = Avalonia.Controls.Grid;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class AutoSelectionPanel : UserControl
    {
        private static readonly Dictionary<Type, List<ModifierEntry>> _cache = new();

        public static readonly StyledProperty<Type> PanelOwnerProperty =
            AvaloniaProperty.Register<AutoSelectionPanel, Type>(nameof(PanelOwner));

        public static readonly StyledProperty<Type> InterfaceProperty =
            AvaloniaProperty.Register<AutoSelectionPanel, Type>(nameof(Interface));

        public Type PanelOwner
        {
            get => GetValue(PanelOwnerProperty);
            set => SetValue(PanelOwnerProperty, value);
        }

        public Type Interface
        {
            get => GetValue(InterfaceProperty);
            set => SetValue(InterfaceProperty, value);
        }

        static AutoSelectionPanel()
        {
            PanelOwnerProperty.Changed.AddClassHandler<AutoSelectionPanel>((panel, e) => panel.Discover());
            InterfaceProperty.Changed.AddClassHandler<AutoSelectionPanel>((panel, e) => panel.Discover());
        }

        public AutoSelectionPanel()
        {
            InitializeComponent();
        }

        private void Discover()
        {
            var key = PanelOwner ?? Interface;
            if (key == null) return;

            if (!_cache.TryGetValue(key, out var entries))
            {
                entries = PanelOwner != null
                    ? DiscoverByAttribute(PanelOwner)
                    : DiscoverByInterface(Interface);
                _cache[key] = entries;
            }

            int columns = entries.Count switch { <= 4 => 1, <= 9 => 2, _ => 3 };
            int rows = (int)Math.Ceiling((double)entries.Count / columns);

            var grid = new AvaloniaGrid { Margin = new Thickness(4) };

            for (int i = 0; i < columns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int col = 0; col < columns; col++)
            {
                var stack = new StackPanel();
                AvaloniaGrid.SetColumn(stack, col);

                var columnEntries = entries
                    .Skip(col * rows)
                    .Take(rows);

                foreach (var entry in columnEntries)
                {
                    var button = new Button
                    {
                        Content = entry.Label,
                        Margin = new Thickness(0),
                        CommandParameter = entry.Type
                    };
                    button[!TemplatedControl.ThemeProperty] = new DynamicResourceExtension("ButtonItemControl");
                    button.Bind(Button.CommandProperty, new Binding("AddItemCommand"));
                    stack.Children.Add(button);
                }

                grid.Children.Add(stack);
            }

            Content = grid;
        }

        private static List<ModifierEntry> DiscoverByAttribute(Type ownerType)
        {
            return typeof(ModifierPanelAttribute).Assembly
                .GetTypes()
                .Concat(ownerType.Assembly.GetTypes())
                .Distinct()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttributes<ModifierPanelAttribute>()
                    .Any(a => a.PanelOwner == ownerType))
                .Select(t => new ModifierEntry { Type = t })
                .OrderBy(e => e.Label)
                .ToList();
        }

        private static List<ModifierEntry> DiscoverByInterface(Type interfaceType)
        {
            return interfaceType.Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => interfaceType.IsAssignableFrom(t))
                .Select(t => new ModifierEntry { Type = t })
                .OrderBy(e => e.Label)
                .ToList();
        }
    }
}
