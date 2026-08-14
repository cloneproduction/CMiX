// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using CMiX.Core.BaseControls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class PathSelector : UserControl
    {
        public PathSelector()
        {
            InitializeComponent();

            // Replaces the WPF gong drop handler with Avalonia file drop wiring.
            DragDrop.SetAllowDrop(pathComboBox, true);
            pathComboBox.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            pathComboBox.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files)
                ? DragDropEffects.Copy | DragDropEffects.Move
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (DataContext is not AssetSelector assetSelector)
                return;

            if (!e.Data.Contains(DataFormats.Files))
                return;

            var assets = (e.Data.GetFiles() ?? Enumerable.Empty<IStorageItem>())
                .Select(f => f.TryGetLocalPath())
                .Where(p => p != null && File.Exists(p))
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
