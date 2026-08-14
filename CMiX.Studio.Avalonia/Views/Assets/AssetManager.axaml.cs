// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class AssetManager : UserControl
    {
        public AssetManager()
        {
            InitializeComponent();

            // Replaces the WPF gong drop handlers with Avalonia file drop wiring.
            foreach (var target in new Control[] { dropBorder, imagesListBox, videosListBox, geometriesListBox })
            {
                DragDrop.SetAllowDrop(target, true);
                target.AddHandler(DragDrop.DragOverEvent, OnDragOver);
                target.AddHandler(DragDrop.DropEvent, OnDrop);
            }
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
            if (DataContext is not Core.Assets.AssetManager assetManager)
                return;

            if (!e.Data.Contains(DataFormats.Files))
                return;

            var paths = e.Data.GetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => p != null) ?? Enumerable.Empty<string>();
            foreach (var path in paths)
            {
                if (File.Exists(path)) assetManager.CreateAssetFromPath(path);
                if (Directory.Exists(path)) assetManager.CreateAssetFromDirectory(new DirectoryInfo(path));
            }
            e.Handled = true;
        }
    }
}
