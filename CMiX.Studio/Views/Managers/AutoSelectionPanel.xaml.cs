// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CMiX.Core.Prefabs;
using WpfGrid = System.Windows.Controls.Grid;

namespace CMiX.Studio.Views
{
    public partial class AutoSelectionPanel : UserControl
    {
        private static readonly Dictionary<Type, List<ModifierEntry>> _cache = new();

        public static readonly DependencyProperty PanelOwnerProperty =
            DependencyProperty.Register(nameof(PanelOwner), typeof(Type), typeof(AutoSelectionPanel),
                new PropertyMetadata(null, OnDiscoveryChanged));

        public static readonly DependencyProperty InterfaceProperty =
            DependencyProperty.Register(nameof(Interface), typeof(Type), typeof(AutoSelectionPanel),
                new PropertyMetadata(null, OnDiscoveryChanged));

        public Type PanelOwner
        {
            get => (Type)GetValue(PanelOwnerProperty);
            set => SetValue(PanelOwnerProperty, value);
        }

        public Type Interface
        {
            get => (Type)GetValue(InterfaceProperty);
            set => SetValue(InterfaceProperty, value);
        }
        
        public AutoSelectionPanel()
        {
            InitializeComponent();
        }

        private static void OnDiscoveryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoSelectionPanel)d).Discover();
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

            var grid = new WpfGrid { Margin = new Thickness(4) };

            for (int i = 0; i < columns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int col = 0; col < columns; col++)
            {
                var stack = new StackPanel();
                WpfGrid.SetColumn(stack, col);

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
                    button.SetResourceReference(StyleProperty, "ButtonItemControl");
                    button.SetBinding(Button.CommandProperty, new Binding("AddItemCommand"));
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
