// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // What the behaviors of the new tree share: the named controls of the view, its lifetime, its state and its Add panel.
    internal sealed class ListTreeContext
    {
        public ListTreeContext(ListTreeProject owner, PanelListTreeRows rows, ScrollViewer scroller, KeyHintStrip hintStrip, Control endRoomSpace,
            TreeViewLifetime lifetime, ListTreeViewState state, ListTreeFocus focus, AddPanel addPanel, ListTreeMenuOpen menuOpen)
        {
            Owner = owner;
            Rows = rows;
            Scroller = scroller;
            HintStrip = hintStrip;
            EndRoomSpace = endRoomSpace;
            Lifetime = lifetime;
            State = state;
            Focus = focus;
            AddPanel = addPanel;
            MenuOpen = menuOpen;
        }

        public ListTreeProject Owner { get; }

        public PanelListTreeRows Rows { get; }

        public ScrollViewer Scroller { get; }

        public KeyHintStrip HintStrip { get; }

        public Control EndRoomSpace { get; }

        public TreeViewLifetime Lifetime { get; }

        public ListTreeViewState State { get; }

        public ListTreeFocus Focus { get; }

        public AddPanel AddPanel { get; }

        public ListTreeMenuOpen MenuOpen { get; }

        public T Part<T>() where T : ListTreeBehavior => Interaction.GetBehaviors(Owner).OfType<T>().Single();
    }
}
