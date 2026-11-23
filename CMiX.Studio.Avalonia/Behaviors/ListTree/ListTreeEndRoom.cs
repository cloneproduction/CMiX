// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Room under the rows of the new tree lets the view stay where it was when a close takes rows away, as TreeRowMetrics.SetEndRoom gives the old tree.
    internal sealed class ListTreeEndRoom : ListTreeBehavior
    {
        private const int RowsInView = 3;

        protected override void OnWatch(ProjectTree tree) => Context.Scroller.AddHandler(ScrollViewer.ScrollChangedEvent, OnScrollChanged);

        protected override void OnUnwatch() => Context.Scroller.RemoveHandler(ScrollViewer.ScrollChangedEvent, OnScrollChanged);

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e) => SetEndRoom();

        public void SetEndRoom()
        {
            var rows = Context.Rows;
            var scroller = Context.Scroller;
            var space = Context.EndRoomSpace;
            var pitch = rows.RowHeight + rows.RowGap;
            var content = rows.DesiredSize.Height;
            var room = content > scroller.Viewport.Height + 0.5 ? Math.Max(0, scroller.Viewport.Height - RowsInView * pitch) : 0;
            if (Math.Abs(space.Height - room) > 0.5)
                space.Height = room;
        }
    }
}
