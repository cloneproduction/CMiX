// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Notices
{
    // Shows a short message at the bottom of the main window. The message hides itself and does not stop the work of the user.
    // A new message replaces the message that shows.
    public interface IStatusNotice
    {
        void Show(string text);

        // Shows the text with the key of Edit.Undo, so the user sees how to undo the action.
        void ShowWithUndo(string text);
    }
}
