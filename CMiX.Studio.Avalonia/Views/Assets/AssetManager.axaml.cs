// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using HanumanInstitute.MvvmDialogs;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class AssetManager : UserControl
    {
        public AssetManager()
        {
            InitializeComponent();

            foreach (var target in new Control[] { dropBorder, imagesListBox, videosListBox, geometriesListBox, imageSequencesListBox })
            {
                DragDrop.SetAllowDrop(target, true);
                target.AddHandler(DragDrop.DragOverEvent, OnDragOver);
                target.AddHandler(DragDrop.DropEvent, OnDrop);
            }
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files)
                ? DragDropEffects.Copy | DragDropEffects.Move
                : DragDropEffects.None;
            e.Handled = true;
        }

        private async void OnDrop(object? sender, DragEventArgs e)
        {
            if (DataContext is not Core.Assets.AssetManager assetManager)
                return;

            if (!e.Data.Contains(DataFormats.Files))
                return;

            var paths = e.Data.GetFiles()?.Select(f => f.TryGetLocalPath()).Where(p => p != null).Select(p => p!).ToList() ?? new List<string>();
            e.Handled = true;

            try
            {
                // The filesystem walk (deep folders, network shares) runs off the UI thread. Only the
                // AssetRepository mutating calls, which touch bound ObservableCollections, go back through
                // the dispatcher.
                var filePaths = await Task.Run(() =>
                {
                    var result = new List<string>();
                    foreach (var path in paths)
                    {
                        if (File.Exists(path))
                            result.Add(path);
                        else if (Directory.Exists(path))
                            result.AddRange(assetManager.EnumerateAssetFilePaths(new DirectoryInfo(path)));
                    }
                    return result;
                });

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var filePath in filePaths)
                        assetManager.CreateAssetFromPath(filePath);
                });
            }
            catch (Exception ex)
            {
                var dialogService = App.DialogService;
                if (dialogService != null)
                    await dialogService.ShowMessageBoxAsync(assetManager, ex.Message, "Add Assets");
            }
        }
    }
}
