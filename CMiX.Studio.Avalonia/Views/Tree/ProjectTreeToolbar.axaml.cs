// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Popups;

namespace CMiX.Studio.Avalonia.Views
{
    // A click opens the Add panel under the pointer, as Tree.Add does. A keyboard click opens it under the button.
    public partial class ProjectTreeToolbar : UserControl
    {
        private readonly AddPanel panel = new();

        private Point lastPointer;

        public ProjectTreeToolbar()
        {
            InitializeComponent();
            AddHandler(PointerMovedEvent, (_, e) => lastPointer = e.GetPosition(this), RoutingStrategies.Tunnel, handledEventsToo: true);
            AddHandler(PointerPressedEvent, (_, e) => lastPointer = e.GetPosition(this), RoutingStrategies.Tunnel, handledEventsToo: true);
        }

        // For the tests.
        internal AddPanel Panel => panel;

        // The header names the node, because the button is not on a row.
        private void OnAddClick(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not ProjectTree tree)
                return;

            var node = tree.TargetNode;
            var header = node == null ? null : "Add to " + node.Header;
            if (AddButton.IsPointerOver)
                panel.OpenAtPoint(this, lastPointer, tree.GetAddGroups(node), header);
            else
                panel.Open(AddButton, PopupSide.Bottom, tree.GetAddGroups(node), header);
        }
    }
}
