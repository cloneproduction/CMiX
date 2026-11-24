// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The new list tree pushes its pointer and its focus into its view state, and the hooks of TreeViewSet read that state.
    public class ListTreeViewStateInputTests : IDisposable
    {
        private readonly Scene scene = CreateScene([2], s => AddKinds(s.Layer));
        private readonly Window window;
        private readonly ListTreeProject view;
        private readonly Button outside = new() { Content = "Outside" };
        private readonly TextBox outsideText = new();

        public ListTreeViewStateInputTests()
        {
            view = new ListTreeProject { DataContext = scene.Tree };
            view.Rows.MoveDuration = TimeSpan.Zero;
            var panel = new DockPanel();
            DockPanel.SetDock(outside, Dock.Bottom);
            DockPanel.SetDock(outsideText, Dock.Bottom);
            panel.Children.Add(outside);
            panel.Children.Add(outsideText);
            panel.Children.Add(view);
            window = new Window { Content = panel, Width = 400, Height = 900 };
            window.Show();
            Settle();
        }

        public void Dispose() => window.Close();

        private ListTreeViewState State => view.State;

        private TreeNode Layer => scene.LayerNodeOfA(0);

        private void Settle() => ListTreeTestScreen.Settle(window);

        private Point CenterOf(Control control) =>
            control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;

        private void Hover(Control control)
        {
            TreeDrag.Render(window);
            window.MouseMove(CenterOf(control));
            Settle();
        }

        private void Focus(Control control)
        {
            control.Focus();
            Settle();
        }

        [AvaloniaFact]
        public void PointerMove_OntoARowAndAway_SetsAndClearsPointerOverTree()
        {
            Hover(view.Rows.RowOf(Layer)!);
            var over = State.IsPointerOverTree;

            Hover(outside);

            Assert.True(over);
            Assert.False(State.IsPointerOverTree);
            Assert.False(scene.Tree.Views.IsPointerOverTree());
        }

        [AvaloniaFact]
        public void Focus_ARow_SetsTreeFocusedAndPanelFocused()
        {
            Focus(view.Rows.RowOf(Layer)!);

            Assert.True(State.IsTreeFocused);
            Assert.True(State.IsPanelFocused);
            Assert.False(State.IsFocusInOtherPanel);
            Assert.False(State.IsTextBoxFocused);
            Assert.True(scene.Tree.Views.IsTreeFocused());
        }

        [AvaloniaFact]
        public void Focus_AButtonOutside_SetsFocusInOtherPanelAndClearsTreeFocused()
        {
            Focus(view.Rows.RowOf(Layer)!);

            Focus(outside);

            Assert.False(State.IsTreeFocused);
            Assert.False(State.IsPanelFocused);
            Assert.True(State.IsFocusInOtherPanel);
            Assert.True(scene.Tree.Views.IsFocusInOtherPanel());
        }

        [AvaloniaFact]
        public void Focus_ATextBox_SetsTextBoxFocused()
        {
            Focus(outsideText);

            Assert.True(State.IsTextBoxFocused);
            Assert.True(scene.Tree.Views.IsTextBoxFocused());
        }
    }
}
