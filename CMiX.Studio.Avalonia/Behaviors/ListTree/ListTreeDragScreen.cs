// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The cursor of the rows and the note under them during a drag of the new tree.
    internal sealed class ListTreeDragScreen
    {
        public const StandardCursorType MoveCursorType = StandardCursorType.DragMove;

        private readonly Cursor moveCursor = new(MoveCursorType);
        private readonly ScrollViewer scroller;
        private readonly TextBlock note;
        private Cursor? cursorBefore;

        public ListTreeDragScreen(ScrollViewer scroller, TextBlock note)
        {
            this.scroller = scroller;
            this.note = note;
        }

        public void ShowMoveCursor()
        {
            cursorBefore = scroller.Cursor;
            scroller.Cursor = moveCursor;
        }

        public void RestoreCursor()
        {
            scroller.Cursor = cursorBefore;
            cursorBefore = null;
        }

        public void ShowNote(string? text)
        {
            note.Text = text;
            note.IsVisible = text != null;
        }
    }
}
