// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactivity;

namespace CMiX.UI.Behaviors
{
    // What a drop accepts: the files of the drag, or the folders of the drag.
    public enum FileDropMode
    {
        Files,
        Folders
    }

    public class PathsDroppedEventArgs : EventArgs
    {
        public PathsDroppedEventArgs(IReadOnlyList<string> paths)
        {
            Paths = paths;
        }

        public IReadOnlyList<string> Paths { get; }
    }

    // Lets a control accept files dragged from the file explorer. The behavior only finds the
    // paths. What to do with them is up to the one who handles PathsDropped, or who overrides
    // OnPathsDropped.
    public class FileDropBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<FileDropMode> ModeProperty =
            AvaloniaProperty.Register<FileDropBehavior, FileDropMode>(nameof(Mode), FileDropMode.Files);

        public FileDropMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public static readonly StyledProperty<DragDropEffects> DragEffectsProperty =
            AvaloniaProperty.Register<FileDropBehavior, DragDropEffects>(nameof(DragEffects), DragDropEffects.Copy);

        // The effect shown over the control while files are dragged on it.
        public DragDropEffects DragEffects
        {
            get => GetValue(DragEffectsProperty);
            set => SetValue(DragEffectsProperty, value);
        }

        public event EventHandler<PathsDroppedEventArgs>? PathsDropped;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject == null)
                return;

            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
        }

        protected virtual void OnPathsDropped(IReadOnlyList<string> paths)
        {
            PathsDropped?.Invoke(this, new PathsDroppedEventArgs(paths));
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragEffects : DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files))
                return;

            var folders = Mode == FileDropMode.Folders;
            var paths = (e.Data.GetFiles() ?? Enumerable.Empty<IStorageItem>())
                .Select(item => item.TryGetLocalPath())
                .Where(path => path != null && (folders ? Directory.Exists(path) : File.Exists(path)))
                .Select(path => path!)
                .ToList();
            if (paths.Count == 0)
                return;

            OnPathsDropped(paths);
            e.Handled = true;
        }
    }
}
