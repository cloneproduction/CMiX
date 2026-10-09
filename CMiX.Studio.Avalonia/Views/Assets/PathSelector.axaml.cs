// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.Studio.Avalonia.Views.Controls;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class PathSelector : CaptionedUserControl
    {
        static PathSelector()
        {
            CaptionProperty.OverrideDefaultValue<PathSelector>("Filename");
        }

        public PathSelector()
        {
            InitializeComponent();
            DragDrop.SetAllowDrop(pathComboBox, true);
            pathComboBox.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            pathComboBox.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files)
                ? DragDropEffects.Copy | DragDropEffects.Move
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (DataContext is not AssetSelector assetSelector)
                return;

            if (!e.Data.Contains(DataFormats.Files))
                return;

            var assets = (e.Data.GetFiles() ?? Enumerable.Empty<IStorageItem>())
                .Select(f => f.TryGetLocalPath())
                .Where(p => p != null && (SelectsFolder ? Directory.Exists(p) : File.Exists(p)))
                .Select(assetSelector.CreateAssetFromPath)
                .Where(a => a != null)
                .ToList();

            foreach (var asset in assets)
                assetSelector.AssetRepository.Add(asset);

            var first = assets.FirstOrDefault();
            if (first == null) return;

            assetSelector.Asset = first;
            assetSelector.FilePath.Value = first.FilePath;
            e.Handled = true;
        }

        private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not AssetSelector assetSelector)
                return;

            var path = SelectsFolder ? await BrowseFolder(assetSelector) : await BrowseFile(assetSelector);
            if (string.IsNullOrWhiteSpace(path))
                return;

            assetSelector.SetAssetFromPath(path);
        }

        private static async Task<string?> BrowseFile(AssetSelector assetSelector)
        {
            var file = await App.DialogService!.ShowOpenFileDialogAsync(assetSelector, new OpenFileDialogSettings
            {
                Filters = new List<FileFilter> { new FileFilter("Assets", AssetTypes.AllExtensions) }
            });

            return file?.LocalPath;
        }

        private static async Task<string?> BrowseFolder(AssetSelector assetSelector)
        {
            var folder = await App.DialogService!.ShowOpenFolderDialogAsync(assetSelector, new OpenFolderDialogSettings());

            return folder?.LocalPath;
        }

        public static readonly StyledProperty<bool> SelectsFolderProperty =
            AvaloniaProperty.Register<PathSelector, bool>(nameof(SelectsFolder));
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
    }
}
