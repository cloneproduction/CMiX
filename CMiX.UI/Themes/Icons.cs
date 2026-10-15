// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Material.Icons;

namespace CMiX.UI.Themes
{
    // Semantic key to MaterialIconKind, read by AppIcon. This class holds the generic keys.
    // An application adds its own keys with Register.
    public static class Icons
    {
        private static readonly Dictionary<string, MaterialIconKind> _kinds = new Dictionary<string, MaterialIconKind>
        {
            { "Copy", MaterialIconKind.ContentCopy },
            { "Paste", MaterialIconKind.ContentPaste },
            { "Reset", MaterialIconKind.Restore },
            { "AddTab", MaterialIconKind.TabPlus },
            { "CloseTab", MaterialIconKind.TabRemove },
            { "Edit", MaterialIconKind.Pencil },
            { "Folder", MaterialIconKind.Folder },
            { "Folder_Opened", MaterialIconKind.FolderOpen },
            { "Add", MaterialIconKind.Plus },
            { "Sub", MaterialIconKind.Minus },
            { "Rename", MaterialIconKind.RenameBox },
            { "Close", MaterialIconKind.Close },
            { "IsChecked", MaterialIconKind.Check },
            { "Maximize", MaterialIconKind.WindowMaximize },
            { "Minimize", MaterialIconKind.WindowMinimize },
            { "Warning", MaterialIconKind.Alert },
            { "Settings", MaterialIconKind.Cog },
            { "Menu", MaterialIconKind.Menu },
            { "Play", MaterialIconKind.Play },
            { "Stop", MaterialIconKind.Stop },
            { "Visible", MaterialIconKind.Eye },
            { "NotVisible", MaterialIconKind.EyeOff },
            { "Power", MaterialIconKind.Power },
            { "Update", MaterialIconKind.PowerCycle },
            { "DragHandle", MaterialIconKind.DragHorizontal },
            { "ArrowUp", MaterialIconKind.ArrowUp },
            { "ArrowDown", MaterialIconKind.ArrowDown },
            { "ArrowRight", MaterialIconKind.ArrowRight },
            { "ArrowLeft", MaterialIconKind.ArrowLeft },
            { "ChevronUp", MaterialIconKind.ChevronUp },
            { "ChevronDown", MaterialIconKind.ChevronDown },
            { "ChevronRight", MaterialIconKind.ChevronRight },
            { "ChevronLeft", MaterialIconKind.ChevronLeft },
            { "Unlinked", MaterialIconKind.CircleMedium },
        };

        public static IReadOnlyDictionary<string, MaterialIconKind> Kinds => _kinds;

        // Registering the same key with the same kind again does nothing. A different kind throws.
        public static void Register(string key, MaterialIconKind kind)
        {
            if (_kinds.TryGetValue(key, out var existing))
            {
                if (existing != kind)
                    throw new InvalidOperationException($"The icon key '{key}' is already registered as {existing}.");

                return;
            }

            _kinds.Add(key, kind);
        }
    }
}
