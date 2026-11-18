// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using CMiX.Studio.Avalonia.Converters;

namespace CMiX.Studio.Avalonia.Views
{
    public enum TreeRowBrushRole
    {
        Icon,
        Text,
    }

    public static class TreeRowBrush
    {
        private static readonly KindIconBrushConverter Converter = new();

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
                element.Bind(TextElement.ForegroundProperty, CreateBrushBinding(role, e.Priority));
        }

        // The brush has the priority of the role, so a role that a style sets gives a style value.
        private static MultiBinding CreateBrushBinding(TreeRowBrushRole role, BindingPriority priority) => new()
        {
            Converter = Converter,
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
