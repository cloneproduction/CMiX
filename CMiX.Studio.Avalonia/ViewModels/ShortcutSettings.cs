// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Input;
using CMiX.Studio.Avalonia.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels
{
    // The settings tab edits the shortcut map with this. Each change goes into the map and into the file.
    public sealed class ShortcutSettings : ObservableObject
    {
        // The labels that the settings tab shows. A name that is not here shows the part after the dot.
        private static readonly IReadOnlyDictionary<string, string> Labels = new Dictionary<string, string>
        {
            ["Edit.Undo"] = "Undo",
            ["Edit.Redo"] = "Redo",
            ["Edit.RedoAlt"] = "Redo (alternative)",
            ["File.Open"] = "Open project",
            ["Window.NextPanel"] = "Next panel",
            ["Window.PreviousPanel"] = "Previous panel",
            ["Window.FocusPanel1"] = "Panel 1",
            ["Window.FocusPanel2"] = "Panel 2",
            ["Window.FocusPanel3"] = "Panel 3",
            ["Window.FocusPanel4"] = "Panel 4",
            ["Tree.Focus"] = "Focus the tree",
            ["Tree.Add"] = "Add",
            ["Tree.Menu"] = "Context menu",
            ["Tree.Expand"] = "Expand or collapse",
            ["Tree.CollapseOthers"] = "Collapse others",
            ["Tree.ExpandBranch"] = "Expand the branch",
            ["Tree.CloseLevel"] = "Close one level",
            ["Tree.OpenLevel"] = "Open one level",
            ["Tree.CollapseAll"] = "Collapse all",
            ["Tree.Search"] = "Search",
            ["Tree.ClearSearch"] = "Clear the search",
            ["Tree.Rename"] = "Rename",
            ["Tree.Remove"] = "Remove",
            ["Tree.Duplicate"] = "Duplicate",
            ["Tree.MoveUp"] = "Move up",
            ["Tree.MoveDown"] = "Move down",
            ["Tree.Open"] = "Open",
            ["Tree.Reveal"] = "Reveal the selected row",
            ["Tree.Hide"] = "Hide or show",
            ["Tree.ShowAll"] = "Show all rows",
            ["Tree.Grab"] = "Grab the row",
        };

        private readonly ShortcutMap map;
        private readonly List<ShortcutRow> rows = new();

        public ShortcutSettings(ShortcutMap map)
        {
            this.map = map;

            Scopes = map.Names
                .GroupBy(ShortcutMap.ScopeOf)
                .Select(group => new ShortcutScope(group.Key, group.Select(name => new ShortcutRow(this, name, LabelOf(name))).ToList()))
                .ToList();
            rows.AddRange(Scopes.SelectMany(scope => scope.Rows));

            ResetAllCommand = new RelayCommand(ResetAll);
            map.PropertyChanged += OnMapChanged;
        }

        public IReadOnlyList<ShortcutScope> Scopes { get; }

        public IRelayCommand ResetAllCommand { get; }

        private string? saveError;

        // The message of the last save that failed. Null when the last save worked.
        public string? SaveError
        {
            get => saveError;
            private set => SetProperty(ref saveError, value);
        }

        public static string LabelOf(string name)
        {
            if (Labels.TryGetValue(name, out var label))
                return label;

            var dot = name.IndexOf('.');
            return dot < 0 ? name : name[(dot + 1)..];
        }

        internal KeyGesture? GestureOf(string name) => map[name];

        internal bool IsConflict(string name) => map.Conflicts.Contains(name);

        // The labels of the other names with the same gesture that can conflict with this name. Null when there is no conflict.
        internal string? ConflictTextOf(string name)
        {
            var gesture = map[name];
            if (gesture == null)
                return null;

            var others = map.Names
                .Where(other => other != name && ShortcutMap.CanConflict(name, other) && Equals(map[other]?.ToString(), gesture.ToString()))
                .Select(LabelOf)
                .ToList();

            return others.Count == 0 ? null : $"{gesture} is also the gesture of {string.Join(", ", others)}.";
        }

        internal void SetGesture(string name, KeyGesture? gesture)
        {
            if (Equals(map[name], gesture))
                return;

            map.Set(name, gesture);
            Save();
        }

        internal void Reset(string name)
        {
            map.Reset(name);
            Save();
        }

        private void ResetAll()
        {
            map.ResetAll();
            Save();
        }

        // A failed save keeps the change in the map. The next good save clears the error.
        private void Save()
        {
            try
            {
                map.Save();
                SaveError = null;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                SaveError = $"The shortcuts are not saved: {ex.Message}";
            }
        }

        private void OnMapChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Item[]")
                return;

            foreach (var row in rows)
                row.Refresh();
        }
    }

    // The rows of one scope, for example "Tree".
    public sealed class ShortcutScope
    {
        public ShortcutScope(string name, IReadOnlyList<ShortcutRow> rows)
        {
            Name = name;
            Rows = rows;
        }

        public string Name { get; }

        public IReadOnlyList<ShortcutRow> Rows { get; }
    }

    // One command of the shortcut map.
    public sealed class ShortcutRow : ObservableObject
    {
        private readonly ShortcutSettings owner;

        internal ShortcutRow(ShortcutSettings owner, string name, string label)
        {
            this.owner = owner;
            Name = name;
            Label = label;
            ResetCommand = new RelayCommand(() => owner.Reset(Name));
        }

        public string Name { get; }

        public string Label { get; }

        public KeyGesture? Gesture
        {
            get => owner.GestureOf(Name);
            set => owner.SetGesture(Name, value);
        }

        public bool IsConflict => owner.IsConflict(Name);

        public string? ConflictText => owner.ConflictTextOf(Name);

        public IRelayCommand ResetCommand { get; }

        internal void Refresh()
        {
            OnPropertyChanged(nameof(Gesture));
            OnPropertyChanged(nameof(IsConflict));
            OnPropertyChanged(nameof(ConflictText));
        }
    }
}
