// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CMiX.UI.Controls.Number
{
    // Tab and Shift+Tab in an edit box move the edit mode to the next or the previous value editor.
    // The loop is the nearest parent with EditCycle.IsLoop. Without one, Tab only commits the value.
    // A loop does not include the editors of a loop inside it. The order is the order of the visual tree.
    // The search wraps at both ends.
    public static class EditCycle
    {
        public static readonly AttachedProperty<bool> IsLoopProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsLoop", typeof(EditCycle));

        public static bool GetIsLoop(Control obj) => obj.GetValue(IsLoopProperty);
        public static void SetIsLoop(Control obj, bool value) => obj.SetValue(IsLoopProperty, value);

        // The editor that comes after (or before) the editor that holds the visual. Null when there is none.
        public static IEditableValue? Find(Visual from, bool backwards)
        {
            var current = from.FindAncestorOfType<IEditableValue>(includeSelf: true);
            if (current is not Visual currentVisual)
                return null;

            var loop = Loop(currentVisual);
            if (loop == null)
                return null;

            var editors = Editors(loop).ToList();
            var index = editors.IndexOf(current);
            if (index < 0 || editors.Count < 2)
                return null;

            var step = backwards ? editors.Count - 1 : 1;
            return editors[(index + step) % editors.Count];
        }

        private static Visual? Loop(Visual from)
        {
            foreach (var ancestor in from.GetVisualAncestors())
            {
                if (ancestor is Control control && GetIsLoop(control))
                    return control;
            }

            return null;
        }

        // The current editor stays in the list even if it cannot be edited, so the search finds its place.
        private static IEnumerable<IEditableValue> Editors(Visual loop) =>
            loop.GetVisualDescendants()
                .OfType<IEditableValue>()
                .Where(editor => editor is Visual visual && ReferenceEquals(Loop(visual), loop))
                .Where(editor => editor.IsEditing || IsAvailable(editor));

        private static bool IsAvailable(IEditableValue editor) =>
            editor.CanEdit && editor is Control { IsEffectivelyVisible: true, IsEffectivelyEnabled: true };
    }
}
