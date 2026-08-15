// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Undo;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly UndoManager _undoManager;

        // Resized fires continuously while an edge is dragged, and the descendant walk below is
        // only needed once the drag settles. AttachedToVisualTree does not bubble in Avalonia (each
        // Visual raises it on itself, not on its ancestors), so there is no cheaper way to learn
        // that a new masked Border entered the tree than walking for it, which rules out a plain
        // cache invalidated on tree change. The timer coalesces the whole burst of Resized events
        // from one drag into a single walk instead of one walk per event.
        private readonly DispatcherTimer _resizeMaskRefreshTimer;

        // The DataContext is assigned by App after construction, matching WPF. With
        // the context already present during construction every deferred child
        // binding transiently evaluates against the window context before its local
        // DataContext applies, which logs far more binding noise than the null case.
        public MainWindow(UndoManager undoManager)
        {
            InitializeComponent();
            _undoManager = undoManager;
            titleBar.PointerPressed += TitleBar_PointerPressed;
            Resized += MainWindow_Resized;

            _resizeMaskRefreshTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(150), DispatcherPriority.Background, RefreshResizeMaskBrushes);
            _resizeMaskRefreshTimer.Stop();
        }

        private void MainWindow_Resized(object sender, WindowResizedEventArgs e)
        {
            _resizeMaskRefreshTimer.Stop();
            _resizeMaskRefreshTimer.Start();
        }

        private void RefreshResizeMaskBrushes(object sender, EventArgs e)
        {
            _resizeMaskRefreshTimer.Stop();

            // Recreating the brush forces the compositor to drop and reallocate the cached
            // OpacityMask surface, which InvalidateVisual alone does not trigger.
            foreach (var border in this.GetVisualDescendants().OfType<Border>())
            {
                if (border.OpacityMask is global::Avalonia.Media.VisualBrush visualBrush)
                    border.OpacityMask = new global::Avalonia.Media.VisualBrush { Visual = visualBrush.Visual };
            }
        }

        private void TitleBar_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            // The WPF chrome excluded the menu and the window buttons from the drag
            // area with IsHitTestVisibleInChrome. Starting a move drag here would
            // steal their pointer release and no click would ever complete, so
            // presses inside those interactive children never begin a drag.
            if (e.Source is ILogical source &&
                (source.FindLogicalAncestorOfType<Menus.MainMenu>(true) != null ||
                 source.FindLogicalAncestorOfType<Windows.MainWindowController>(true) != null))
                return;

            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                BeginMoveDrag(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Z && e.KeyModifiers == KeyModifiers.Control)
            {
                _undoManager.Undo();
                e.Handled = true;
            }
            else if (e.Key == Key.Z && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
            {
                _undoManager.Redo();
                e.Handled = true;
            }
            else if (e.Key == Key.Y && e.KeyModifiers == KeyModifiers.Control)
            {
                _undoManager.Redo();
                e.Handled = true;
            }
        }
    }
}
