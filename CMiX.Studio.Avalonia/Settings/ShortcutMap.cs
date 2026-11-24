// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Input;
using CMiX.UI.Converters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.Settings
{
    // The keyboard shortcuts of the Studio: a command name with its gesture. The part of the name before the first dot is the scope.
    // The XAML binds a gesture with the indexer, for example {Binding Shortcuts[Tree.Rename]}.
    public sealed class ShortcutMap : ObservableObject
    {
        public const string FileName = "studio-shortcuts.json";

        // The indexer name that a binding listens to.
        private const string IndexerName = "Item[]";

        public static string DefaultPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CMiX", FileName);

        // An empty gesture: the name has no default gesture. A digit key is written D1. The file also accepts "Ctrl+1".
        private readonly (string Name, string Gesture)[] defaults =
        {
            ("Edit.Undo", "Ctrl+Z"),
            ("Edit.Redo", "Ctrl+Shift+Z"),
            ("Edit.RedoAlt", "Ctrl+Y"),
            ("File.Open", "Ctrl+O"),
            ("Window.NextPanel", "Ctrl+Tab"),
            ("Window.PreviousPanel", "Ctrl+Shift+Tab"),
            ("Window.FocusPanel1", "Ctrl+D1"),
            ("Window.FocusPanel2", "Ctrl+D2"),
            ("Window.FocusPanel3", "Ctrl+D3"),
            ("Window.FocusPanel4", "Ctrl+D4"),
            ("Tree.Focus", ""),
            ("Tree.Add", "Shift+A"),
            ("Tree.Inspect", "Space"),
            ("Tree.Menu", "W"),
            ("Tree.Expand", "E"),
            ("Tree.CollapseOthers", "Alt+E"),
            ("Tree.ExpandBranch", "Shift+E"),
            ("Tree.CloseLevel", "Q"),
            ("Tree.OpenLevel", "Shift+Q"),
            ("Tree.CollapseAll", "Shift+Alt+E"),
            ("Tree.Search", "Ctrl+F"),
            ("Tree.ClearSearch", "Alt+F"),
            ("Tree.Rename", "Ctrl+R"),
            ("Tree.Remove", "Delete"),
            ("Tree.Duplicate", "Shift+D"),
            ("Tree.MoveUp", "Alt+Up"),
            ("Tree.MoveDown", "Alt+Down"),
            ("Tree.Open", "Enter"),
            ("Tree.Reveal", "F"),
            ("Tree.Hide", "H"),
            ("Tree.ShowAll", "Alt+H"),
            ("Tree.Grab", "G"),
        };

        // The defaults of earlier versions. A file that still holds one of them gets the default of now, because the file holds every
        // default since its first save, not only the gestures that the user chose.
        private static readonly (string Name, string Gesture)[] retiredDefaults =
        {
            ("Tree.Duplicate", "Ctrl+D"),
            ("Tree.Rename", "F2"),
        };

        private readonly Dictionary<string, KeyGesture?> gestures = new();

        public ShortcutMap() : this(DefaultPath)
        {
        }

        // The constructor reads no file. Load reads it.
        public ShortcutMap(string filePath)
        {
            FilePath = filePath;
            Names = defaults.Select(entry => entry.Name).ToList();
            foreach (var (name, _) in defaults)
                gestures[name] = DefaultOf(name);
        }

        public string FilePath { get; }

        public IReadOnlyList<string> Names { get; }

        public IReadOnlyDictionary<string, KeyGesture?> Gestures => gestures;

        // An unknown name gives null.
        public KeyGesture? this[string name] => gestures.TryGetValue(name, out var gesture) ? gesture : null;

        // The keys of these scopes work in every scope.
        private static readonly string[] globalScopes = { "Edit", "File", "Window" };

        // The names whose gesture another name also has, in the same scope or in a global scope.
        public IReadOnlyCollection<string> Conflicts => ConflictsOf(gestures);

        public static bool IsGlobalScope(string scope) => globalScopes.Contains(scope, StringComparer.Ordinal);

        internal static IReadOnlyCollection<string> ConflictsOf(IEnumerable<KeyValuePair<string, KeyGesture?>> entries)
        {
            var used = entries.Where(entry => entry.Value != null).ToList();
            return used
                .Where(entry => used.Any(other =>
                    other.Key != entry.Key && other.Value!.Equals(entry.Value) && CanConflict(entry.Key, other.Key)))
                .Select(entry => entry.Key)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();
        }

        // True when the two names are in one scope or one of them is in a global scope.
        public static bool CanConflict(string name, string other)
        {
            var scope = ScopeOf(name);
            var otherScope = ScopeOf(other);
            return scope == otherScope || IsGlobalScope(scope) || IsGlobalScope(otherScope);
        }

        public static string ScopeOf(string name)
        {
            var dot = name.IndexOf('.');
            return dot < 0 ? name : name[..dot];
        }

        // An unknown name does nothing.
        public void Set(string name, KeyGesture? gesture)
        {
            if (!gestures.ContainsKey(name))
                return;

            gestures[name] = gesture;
            RaiseChanged();
        }

        public void Reset(string name) => Set(name, DefaultOf(name));

        public void ResetAll()
        {
            foreach (var name in Names)
                gestures[name] = DefaultOf(name);

            RaiseChanged();
        }

        // Reads the file. A missing file is written with the defaults. A file that can not be written does not stop the Studio.
        // An unknown name or a gesture that does not parse is ignored, and the default stays.
        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                ResetAll();
                try
                {
                    Save();
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException or ArgumentException)
                {
                    // The defaults stay in use.
                }

                return;
            }

            var settings = JsonSettingsFile.Load<ShortcutFile>(FilePath);
            foreach (var name in Names)
                gestures[name] = DefaultOf(name);

            foreach (var (name, text) in settings.Shortcuts ?? new Dictionary<string, string>())
                if (gestures.ContainsKey(name) && !IsRetiredDefault(name, text) && TryParse(text, out var gesture))
                    gestures[name] = gesture;

            RaiseChanged();
        }

        // The file keeps the D form of a digit key, for example "Ctrl+D1".
        public void Save()
        {
            var settings = new ShortcutFile();
            foreach (var name in Names)
                settings.Shortcuts[name] = gestures[name]?.ToString() ?? string.Empty;

            JsonSettingsFile.Save(FilePath, settings);
        }

        private KeyGesture? DefaultOf(string name)
        {
            foreach (var (entry, gesture) in defaults)
                if (entry == name)
                    return gesture.Length == 0 ? null : KeyGestureTextConverter.Parse(gesture);

            return null;
        }

        private static bool IsRetiredDefault(string name, string? text) =>
            text != null && TryParse(text, out var gesture) && gesture != null
            && retiredDefaults.Any(retired => retired.Name == name && KeyGestureTextConverter.Parse(retired.Gesture).Equals(gesture));

        private static bool TryParse(string? text, out KeyGesture? gesture)
        {
            gesture = null;
            if (text == null)
                return false;

            if (text.Trim().Length == 0)
                return true;

            try
            {
                gesture = KeyGestureTextConverter.Parse(text);
                return true;
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidOperationException)
            {
                return false;
            }
        }

        private void RaiseChanged()
        {
            OnPropertyChanged("Item");
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(nameof(Gestures));
            OnPropertyChanged(nameof(Conflicts));
        }
    }
}
