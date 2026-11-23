// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia;
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
        public ListTreeContext(ListTreeProject owner)
        {
            Owner = owner;
            Rows = owner.Rows;
            Scroller = owner.Scroller;
            HintStrip = owner.HintStrip;
            EndRoomSpace = owner.EndRoomSpace;
            DragNote = owner.DragNote;
            Lifetime = new TreeViewLifetime();
            State = new ListTreeViewState();
            Focus = new ListTreeFocus(owner);
            AddPanel = new AddPanel();
        }

        public ListTreeProject Owner { get; }

        public PanelListTreeRows Rows { get; }

        public ScrollViewer Scroller { get; }

        public KeyHintStrip HintStrip { get; }

        public Control EndRoomSpace { get; }

        public TextBlock DragNote { get; }

        public TreeViewLifetime Lifetime { get; }

        public ListTreeViewState State { get; }

        public ListTreeFocus Focus { get; }

        public AddPanel AddPanel { get; }

        public ListTreeInputOwner InputOwner { get; set; }

        public bool IsMenuOpen => Scroller.ContextMenu?.IsOpen == true || AddPanel.IsOpen;

        // The offset of the scroller is read now, so the point is right also in a scroll that the layout has not shown yet.
        public ListTreePoint ToRows(Point inScroller) => new(inScroller.X - Rows.Bounds.X, inScroller.Y + Scroller.Offset.Y - Rows.Bounds.Y);

        public double ClampOffset(double y) => Math.Clamp(y, 0, Math.Max(0, Scroller.Extent.Height - Scroller.Viewport.Height));

        public void ScrollTo(double y) => Scroller.Offset = Scroller.Offset.WithY(ClampOffset(y));

        public T Part<T>() where T : ListTreeBehavior => Interaction.GetBehaviors(Owner).OfType<T>().Single();
    }
}
