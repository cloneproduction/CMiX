// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.Path
{
    public class PathEventArgs : EventArgs
    {
        public PathEventArgs(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }

    // Chooses a file or a folder: a list of known items, a Browse button, and a drop of files from
    // the file explorer. The control only finds the path. What the path becomes is up to the one
    // who handles PathChosen and PathsDropped, or who overrides OnPathChosen and OnPathsDropped.
    // The items of the list need a Name.
    // Inside this namespace write System.IO.Path, because the namespace hides the name Path.
    public partial class PathSelector : UserControlCaptioned
    {
        private readonly FileDropBehavior _drop;

        static PathSelector()
        {
            CaptionProperty.OverrideDefaultValue<PathSelector>("Filename");
            SelectsFolderProperty.Changed.AddClassHandler<PathSelector>((selector, e) => selector.UpdateDropMode());
        }

        public PathSelector()
        {
            InitializeComponent();

            _drop = new FileDropBehavior { DragEffects = DragDropEffects.Copy | DragDropEffects.Move };
            _drop.PathsDropped += (s, e) => OnPathsDropped(e.Paths);
            Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(pathComboBox).Add(_drop);
            UpdateDropMode();
        }

        public event EventHandler<PathEventArgs>? PathChosen;

        public event EventHandler<PathsDroppedEventArgs>? PathsDropped;

        // Same as the user choosing this path with Browse.
        public void ChoosePath(string path) => OnPathChosen(path);

        protected virtual void OnPathChosen(string path)
        {
            PathChosen?.Invoke(this, new PathEventArgs(path));
        }

        protected virtual void OnPathsDropped(IReadOnlyList<string> paths)
        {
            PathsDropped?.Invoke(this, new PathsDroppedEventArgs(paths));
        }

        // The path the user picks in the file dialog, or null. The dialog is the one of the window.
        protected virtual async Task<string?> BrowseAsync()
        {
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage == null)
                return null;

            if (SelectsFolder)
            {
                var folders = await storage.OpenFolderPickerAsync(new FolderPickerOpenOptions { AllowMultiple = false });
                return folders.FirstOrDefault()?.TryGetLocalPath();
            }

            var options = new FilePickerOpenOptions { AllowMultiple = false };
            var patterns = Extensions?.Select(extension => "*." + extension.TrimStart('.')).ToList();
            if (patterns is { Count: > 0 })
                options.FileTypeFilter = new[] { new FilePickerFileType(FileTypeName) { Patterns = patterns } };

            var files = await storage.OpenFilePickerAsync(options);
            return files.FirstOrDefault()?.TryGetLocalPath();
        }

        private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
        {
            var path = await BrowseAsync();
            if (!string.IsNullOrWhiteSpace(path))
                OnPathChosen(path);
        }

        private void UpdateDropMode()
        {
            if (_drop != null)
                _drop.Mode = SelectsFolder ? FileDropMode.Folders : FileDropMode.Files;
        }

        public static readonly StyledProperty<bool> SelectsFolderProperty =
            AvaloniaProperty.Register<PathSelector, bool>(nameof(SelectsFolder));

        // False: the control chooses a file. True: it chooses a folder.
        public bool SelectsFolder
        {
            get => GetValue(SelectsFolderProperty);
            set => SetValue(SelectsFolderProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<PathSelector, IEnumerable>(nameof(ItemsSource));

        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<PathSelector, object>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly StyledProperty<IReadOnlyList<string>?> ExtensionsProperty =
            AvaloniaProperty.Register<PathSelector, IReadOnlyList<string>?>(nameof(Extensions));

        // The file types of the Browse dialog, for example "png" or ".png". Empty means all files.
        public IReadOnlyList<string>? Extensions
        {
            get => GetValue(ExtensionsProperty);
            set => SetValue(ExtensionsProperty, value);
        }

        public static readonly StyledProperty<string> FileTypeNameProperty =
            AvaloniaProperty.Register<PathSelector, string>(nameof(FileTypeName), "Files");

        // The name of the file type filter in the Browse dialog.
        public string FileTypeName
        {
            get => GetValue(FileTypeNameProperty);
            set => SetValue(FileTypeNameProperty, value);
        }
    }
}
