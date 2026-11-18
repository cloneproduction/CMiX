// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The tree captures the pointer, so the moves, the wheel and the release come on every platform.
    public sealed class TreeRowDrag
    {
        // It is the token DragThreshold. A test pins the two.
        public const double Threshold = 4.0;

        public const StandardCursorType MoveCursorType = TreeDragSession.MoveCursorType;

        public static readonly TimeSpan DefaultRestDelay = TreeDragSession.DefaultRestDelay;

        private readonly TreeView treeView;
        private readonly TreeDragSession session;

        private TreeNode? pressed;
        private Point pressPoint;

        private IPointer? pointer;

        internal TreeRowDrag(TreeView treeView, TreeDragSession session)
        {
            this.treeView = treeView;
            this.session = session;
        }

        // A drag that another starter began does not count.
        public bool IsDragging => pointer != null && session.IsRunning;

        // For the tests.
        public Cursor MoveCursor => session.MoveCursor;
        internal TreeDragGhost Ghost => session.Ghost;
        internal IReadOnlyList<TreeNode> OpenedByDrag => session.OpenedRows;
        internal TreeNode? RestingNode => session.RestingNode;
        internal TimeSpan? RestTimerInterval => session.RestTimerInterval;
        internal void OpenRested() => session.OpenRested();

        public void Press(TreeNode? node, Point position)
        {
            pressed = node;
            pressPoint = position;
        }

        public void Move(Point position, PointerEventArgs e)
        {
            if (!IsDragging)
            {
                if (pressed == null || session.IsRunning)
                    return;

                var distance = position - pressPoint;
                if (Math.Abs(distance.X) < Threshold && Math.Abs(distance.Y) < Threshold)
                    return;

                Begin(pressed, e.Pointer, position);
            }

            session.Track(position);
        }

        // The release can come at another point than the last move, so the place is read again.
        public void Release(Point position)
        {
            pressed = null;
            if (!IsDragging)
                return;

            session.Aim(position);
            End(TreeDragEnd.Drop);
        }

        public void Cancel()
        {
            pressed = null;
            End(TreeDragEnd.Cancel);
        }

        public void Wheel(int steps)
        {
            if (IsDragging)
                session.ScrollByWheel(steps);
        }

        private void Begin(TreeNode node, IPointer pointer, Point position)
        {
            pressed = null;
            this.pointer = pointer;
            session.Start(node, position);
            pointer.Capture(treeView);
        }

        // The release of the capture comes back as a Cancel, which then does nothing.
        private void End(TreeDragEnd end)
        {
            if (!IsDragging)
                return;

            session.End(end);

            var captured = pointer;
            pointer = null;
            if (ReferenceEquals(captured?.Captured, treeView))
                captured.Capture(null);
        }
    }
}
