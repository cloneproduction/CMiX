// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    internal static class UndoCounts
    {
        // UndoManager gives no count of its steps, so the test reads its private stack.
        public static int StepsOf(UndoManager undo) =>
            ((LinkedList<IUndoCommand>)typeof(UndoManager).GetField("_undoStack", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(undo)!).Count;
    }
}
