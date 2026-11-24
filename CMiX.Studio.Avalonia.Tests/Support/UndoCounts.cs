// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Reflection;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // UndoManager gives no access to its last step, so LastStepOf reads its private stack.
    internal static class UndoCounts
    {
        public static int StepsOf(UndoManager undo) => undo.StepCount;

        public static int RedoStepsOf(UndoManager undo) => undo.RedoStepCount;

        public static IUndoCommand LastStepOf(UndoManager undo) => UndoStackOf(undo).Last!.Value;

        private static LinkedList<IUndoCommand> UndoStackOf(UndoManager undo) =>
            (LinkedList<IUndoCommand>)FieldOf("_undoStack").GetValue(undo)!;

        private static FieldInfo FieldOf(string name) =>
            typeof(UndoManager).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
}
