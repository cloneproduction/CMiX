// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed class NodeChosenEventArgs : RoutedEventArgs
    {
        public NodeChosenEventArgs(RoutedEvent routedEvent, object source, TreeNode node)
            : base(routedEvent, source)
        {
            Node = node;
        }

        public TreeNode Node { get; }
    }
}
