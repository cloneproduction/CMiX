// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // Room under the rows of the new tree lets the view stay where it was when a close takes rows away, as TreeRowMetrics.SetEndRoom gives the old tree.
    internal sealed class ListTreeEndRoom
    {
        private const int RowsInView = 3;

        private readonly PanelListTreeRows rows;
        private readonly ScrollViewer scroller;
        private readonly Control space;

        public ListTreeEndRoom(PanelListTreeRows rows, ScrollViewer scroller, Control space)
        {
            this.rows = rows;
            this.scroller = scroller;
            this.space = space;
        }

        public void Attach() => scroller.AddHandler(ScrollViewer.ScrollChangedEvent, OnScrollChanged);

        public void Detach() => scroller.RemoveHandler(ScrollViewer.ScrollChangedEvent, OnScrollChanged);

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e) => SetEndRoom();

        public void SetEndRoom()
        {
            var pitch = rows.RowHeight + rows.RowGap;
            var content = rows.DesiredSize.Height;
            var room = content > scroller.Viewport.Height + 0.5 ? Math.Max(0, scroller.Viewport.Height - RowsInView * pitch) : 0;
            if (Math.Abs(space.Height - room) > 0.5)
                space.Height = room;
        }
    }
}
