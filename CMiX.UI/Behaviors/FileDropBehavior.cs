// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace CMiX.UI.Behaviors
{
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

        // The paths of the current drag, classified on DragEnter. Null when no drag is over the control.
        private DropFiles? current;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            AssociatedObject.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            current = null;
            if (AssociatedObject == null)
                return;

            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, OnDrop);
        }

        protected virtual void OnPathsDropped(IReadOnlyList<string> paths)
        {
            PathsDropped?.Invoke(this, new PathsDroppedEventArgs(paths));
        }

        // True when the control takes this path. The behavior calls it once per path and drag, on DragEnter.
        protected virtual bool Accepts(string path) => true;

        // The record of the drag. A drag with other data is classified again.
        private DropFiles FilesOf(DragEventArgs e)
        {
            if (current == null || !ReferenceEquals(current.Data, e.Data))
                current = DropFiles.Classify(e.Data, Mode, Accepts);
            return current;
        }

        private void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
                FilesOf(e);
        }

        private void OnDragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects = e.Data.Contains(DataFormats.Files) && FilesOf(e).IsAccepted ? DragEffects : DragDropEffects.None;
            e.Handled = true;
        }

        // A leave of a child of the control keeps the record.
        private void OnDragLeave(object? sender, DragEventArgs e)
        {
            if (AssociatedObject == null || DropFiles.LeavesControl(AssociatedObject, e))
                current = null;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!e.Data.Contains(DataFormats.Files))
                return;

            var paths = FilesOf(e).Accepted;
            current = null;
            if (paths.Count == 0)
                return;

            OnPathsDropped(paths);
            e.Handled = true;
        }
    }
}
