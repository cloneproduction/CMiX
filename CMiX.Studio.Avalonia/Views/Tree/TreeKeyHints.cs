// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Services;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Converters;

namespace CMiX.Studio.Avalonia.Views
{
    // What the tree does with the keys at this time. The hint strip under the tree shows the keys of the mode.
    public enum TreeHintMode
    {
        // The rows or the panel: no search, no menu.
        Idle,

        // The keyboard focus is in the search field of the panel.
        Search,

        // A search filters the rows, and the keyboard focus is in the tree.
        Filter,

        // A menu or the Add panel of the tree is open.
        Menu,

        // A row follows the pointer until the user places it.
        Grab
    }

    // The hints of the tree for each mode. A gesture of the shortcut map comes from the map, so a changed key shows correctly, and a key
    // with no gesture shows no hint. The other keys are fixed in the code of the tree, the search field and the menus. Their text has the
    // same form as the text of a gesture (KeyGestureTextConverter.TextOf), as in the tooltips and the Settings tab.
    // Idle shows only what fits one line of a panel of the usual width: move, select, open, menu, add and undo.
    public static class TreeKeyHints
    {
        public const string Arrows = "Arrows";
        public const string Click = "Click";
        public const string RightClick = "Right click";
        public static readonly string Escape = TextOf(Key.Escape);
        public static readonly string Enter = TextOf(Key.Enter);
        public static readonly string ShiftEnter = TextOf(Key.Enter, KeyModifiers.Shift);
        public static readonly string Down = TextOf(Key.Down);

        private static string TextOf(Key key, KeyModifiers modifiers = KeyModifiers.None) =>
            KeyGestureTextConverter.TextOf(new KeyGesture(key, modifiers));

        public static IReadOnlyList<KeyHint> For(TreeHintMode mode, ShortcutMap shortcuts)
        {
            IEnumerable<KeyHint?> hints = mode switch
            {
                TreeHintMode.Search => new[]
                {
                    new KeyHint(Enter, "next"),
                    new KeyHint(ShiftEnter, "previous"),
                    new KeyHint(Down, "to tree"),
                    new KeyHint(Escape, "clear")
                },
                TreeHintMode.Filter => new[]
                {
                    new KeyHint(Escape, "clear search"),
                    KeyHint.Of("edit search", shortcuts["Tree.Search"]),
                    new KeyHint(Arrows, "move"),
                    KeyHint.Of("select", shortcuts["Tree.Open"]),
                    KeyHint.Of("menu", shortcuts["Tree.Menu"])
                },
                TreeHintMode.Menu => new[] { new KeyHint(Escape, "close") },
                TreeHintMode.Grab => new[]
                {
                    new KeyHint(new[] { Click, Enter }, "place"),
                    new KeyHint(new[] { RightClick, Escape }, "cancel"),
                    new KeyHint(Arrows, "valid places"),
                    KeyHint.Of("open row", shortcuts["Tree.Expand"])
                },
                _ => new[]
                {
                    new KeyHint(Arrows, "move"),
                    KeyHint.Of("select", shortcuts["Tree.Open"]),
                    KeyHint.Of("open", shortcuts["Tree.Expand"]),
                    KeyHint.Of("menu", shortcuts["Tree.Menu"]),
                    KeyHint.Of("add", shortcuts["Tree.Add"]),
                    KeyHint.Of("inspect", shortcuts["Tree.Inspect"]),
                    KeyHint.Of("undo", shortcuts["Edit.Undo"])
                }
            };

            return hints.OfType<KeyHint>().ToList();
        }
    }
}
