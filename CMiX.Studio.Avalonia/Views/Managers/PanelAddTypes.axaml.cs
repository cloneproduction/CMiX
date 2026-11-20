// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using AvaloniaGrid = Avalonia.Controls.Grid;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    public partial class PanelAddTypes : UserControl
    {
        public static readonly StyledProperty<Type> PanelOwnerProperty =
            AvaloniaProperty.Register<PanelAddTypes, Type>(nameof(PanelOwner));

        public static readonly StyledProperty<Type> InterfaceProperty =
            AvaloniaProperty.Register<PanelAddTypes, Type>(nameof(Interface));

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

        static PanelAddTypes()
        {
            PanelOwnerProperty.Changed.AddClassHandler<PanelAddTypes>((panel, e) => panel.Discover());
            InterfaceProperty.Changed.AddClassHandler<PanelAddTypes>((panel, e) => panel.Discover());
        }

        public PanelAddTypes()
        {
            InitializeComponent();
        }

        private void Discover()
        {
            var key = PanelOwner ?? Interface;
            if (key == null) return;

            // AddableTypes keeps the list of each owner and interface.
            var entries = (PanelOwner != null
                    ? AddableTypes.ForOwner(PanelOwner)
                    : AddableTypes.ForInterface(Interface))
                .Select(type => new ModifierEntry { Type = type })
                .ToList();

            var columns = AddableTypes.Columns(entries);

            var grid = new AvaloniaGrid { Margin = new Thickness(4) };

            for (int i = 0; i < columns.Count; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int col = 0; col < columns.Count; col++)
            {
                var stack = new StackPanel();
                AvaloniaGrid.SetColumn(stack, col);

                foreach (var entry in columns[col])
                {
                    var button = new Button
                    {
                        Content = entry.Label,
                        Margin = new Thickness(0),
                        CommandParameter = entry.Type
                    };
                    button[!TemplatedControl.ThemeProperty] = new DynamicResourceExtension("ButtonItem");
                    button.Bind(Button.CommandProperty, new Binding("AddItemCommand"));
                    stack.Children.Add(button);
                }

                grid.Children.Add(stack);
            }

            Content = grid;
        }

    }
}
