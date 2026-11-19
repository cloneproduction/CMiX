// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.LogicalTree;

namespace CMiX.Studio.Avalonia.Views
{
    public enum TreeRowBrushRole
    {
        Icon,
        Text,
    }

    public static class TreeRowBrush
    {
        private const string ConverterKey = "KindIconBrush";

        public static readonly AttachedProperty<TreeRowBrushRole?> RoleProperty =
            AvaloniaProperty.RegisterAttached<Control, TreeRowBrushRole?>("Role", typeof(TreeRowBrush));

        public static void SetRole(Control element, TreeRowBrushRole? value) => element.SetValue(RoleProperty, value);
        public static TreeRowBrushRole? GetRole(Control element) => element.GetValue(RoleProperty);

        static TreeRowBrush()
        {
            RoleProperty.Changed.AddClassHandler<Control>(OnRoleChanged);
        }

        private static void OnRoleChanged(Control element, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is TreeRowBrushRole role)
                BindWhenConverterIsFound(element, role, e.Priority);
        }

        // A role set in XAML changes before the element is in the logical tree, where no resource is found.
        private static void BindWhenConverterIsFound(Control element, TreeRowBrushRole role, BindingPriority priority)
        {
            if (element.Parent != null && TryBindBrush(element, role, priority))
                return;

            void OnAttached(object? sender, LogicalTreeAttachmentEventArgs args)
            {
                element.AttachedToLogicalTree -= OnAttached;
                TryBindBrush(element, role, priority);
            }

            element.AttachedToLogicalTree += OnAttached;
        }

        private static bool TryBindBrush(Control element, TreeRowBrushRole role, BindingPriority priority)
        {
            if (!element.TryFindResource(ConverterKey, out var resource) || resource is not IMultiValueConverter converter)
                return false;

            element.Bind(TextElement.ForegroundProperty, CreateBrushBinding(converter, role, priority));
            return true;
        }

        // The brush has the priority of the role, so a role that a style sets gives a style value.
        private static MultiBinding CreateBrushBinding(IMultiValueConverter converter, TreeRowBrushRole role, BindingPriority priority) => new()
        {
            Converter = converter,
            ConverterParameter = role.ToString(),
            Priority = priority,
            Bindings =
            {
                ToRow("DataContext"),
                ToRow("DataContext.IsCursor"),
                ToRow(nameof(TreeViewItem.IsSelected)),
                ToRow(nameof(TreeViewItem.ItemCount)),
            },
        };

        private static Binding ToRow(string path) => new(path)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(TreeViewItem) },
        };
    }
}
