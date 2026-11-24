// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Controls.String;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeTestScreen;

namespace CMiX.Studio.Avalonia.Tests
{
    // The menus of the list tree: a key opens at the cursor row, a right click at the pointer.
    public class ListTreeMenusTests : IDisposable
    {
        private sealed record Screen : ListTreeMainScreen
        {
            public Screen(ListTreeMainScreen shown) : base(shown) { }

            public ContextMenu? Menu => View.Menu;
            public RowListTree RowOf(TreeNode node) => Assert.IsType<RowListTree>(View.Rows.RowOf(node));
        }

        private Screen? screen;

        public void Dispose()
        {
            if (screen == null)
                return;

            screen.Menu?.Close();
            screen.View.MenuAddPanel.Close();
            screen.Window.Close();
            Dispatcher.UIThread.RunJobs();
        }

        // One composition with three layers. The first layer is selected. The focus is on its row.
        private Screen Show()
        {
            screen = new Screen(ListTreeMainScreen.ShowWithThreeLayers(PanelChoice.Tree));
            screen.Tree.SelectedNode = screen.LayerNode(0);
            Settle(screen.Window);
            screen.RowOf(screen.LayerNode(0)).Focus();
            Settle(screen.Window);
            return screen;
        }

        // Down moves the cursor away from the selected row. The pointer rests on the last layer, away from the cursor row.
        private static TreeNode MoveCursorDown(Screen screen)
        {
            screen.Window.MouseMove(screen.Window.CenterOf(screen.RowOf(screen.LayerNode(2))));
            screen.Window.Press(PhysicalKey.ArrowDown);
            var cursor = screen.Tree.CursorNode!;
            Assert.NotSame(screen.Tree.SelectedNode, cursor);
            Assert.NotSame(screen.LayerNode(2), cursor);
            return cursor;
        }

        // The left edge of the name and the bottom of the row, in the window.
        private static Point BelowNameOf(Screen screen, TreeNode node)
        {
            var row = screen.RowOf(node);
            var name = row.GetVisualDescendants().OfType<TextBoxEditable>().First();
            return new Point(name.TranslatePoint(default, screen.Window)!.Value.X, row.TranslatePoint(new Point(0, row.Bounds.Height), screen.Window)!.Value.Y);
        }

        // The visible panel of the menu, without the margin of the shadow.
        private static Rect MenuBox(Screen screen)
        {
            var frame = screen.Menu!.GetVisualDescendants().OfType<PopupFrame>().First();
            var origin = frame.TranslatePoint(default, screen.Window)!.Value;
            return new Rect(origin.X + PopupFrame.ShadowMargin.Left, origin.Y + PopupFrame.ShadowMargin.Top,
                frame.Bounds.Width - PopupFrame.ShadowMargin.Left - PopupFrame.ShadowMargin.Right,
                frame.Bounds.Height - PopupFrame.ShadowMargin.Top - PopupFrame.ShadowMargin.Bottom);
        }

        private static string? MenuTitle(Screen screen) => screen.Menu!.Items.OfType<MenuItem>().First().Header as string;

        private static void AssertMenuUnderRow(Screen screen, TreeNode node)
        {
            Assert.True(screen.Menu?.IsOpen);
            Assert.Equal(node.Header, MenuTitle(screen));
            var box = MenuBox(screen);
            var below = BelowNameOf(screen, node);
            Assert.Equal(below.X, box.X, 0.51);
            Assert.Equal(below.Y, box.Y, 0.51);
        }

        [AvaloniaFact]
        public void ShiftA_AfterDown_OpensTheAddPanelAtTheCursorRow()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);

            screen.Window.Press(PhysicalKey.A, RawInputModifiers.Shift);

            var panel = screen.View.MenuAddPanel;
            Assert.True(panel.IsOpen);
            var search = panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");
            var row = screen.Window.CenterOf(screen.RowOf(cursor).Band);
            var field = screen.Window.CenterOf(search);
            Assert.Equal(row.X, field.X, 1.5);
            Assert.Equal(row.Y, field.Y, 1.5);
        }

        [AvaloniaFact]
        public void W_AfterDown_OpensTheMenuUnderTheCursorRowAtTheName()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);

            screen.Window.Press(PhysicalKey.W);

            AssertMenuUnderRow(screen, cursor);
        }

        [AvaloniaFact]
        public void ContextMenuKey_AfterDown_OpensTheMenuUnderTheFocusedRow()
        {
            var screen = Show();
            var cursor = MoveCursorDown(screen);
            Assert.True(screen.RowOf(cursor).IsFocused);

            screen.Window.Press(PhysicalKey.ContextMenu);

            AssertMenuUnderRow(screen, cursor);
        }

        [AvaloniaFact]
        public void RightClick_SelectedRowAfterDown_OpensTheMenuAtThePointerAndMovesTheCursorThere()
        {
            var screen = Show();
            var selected = screen.Tree.SelectedNode!;
            MoveCursorDown(screen);
            var point = screen.Window.CenterOf(screen.RowOf(selected));

            screen.Window.RightClick(point);

            Assert.Same(selected, screen.Tree.CursorNode);
            Assert.Same(selected, screen.Tree.SelectedNode);
            Assert.True(screen.Menu?.IsOpen);
            Assert.Equal(selected.Header, MenuTitle(screen));
            Assert.True(MenuBox(screen).Contains(point));
        }

        [AvaloniaFact]
        public void RightClick_UnderTheRows_OpensTheMenuOfTheProject()
        {
            var screen = Show();
            var last = screen.RowOf(screen.LayerNode(2));
            var under = last.TranslatePoint(new Point(last.Bounds.Width / 2, last.Bounds.Height * 3), screen.Window)!.Value;

            screen.Window.RightClick(under);

            Assert.True(screen.Menu?.IsOpen);
            Assert.Equal("Project", MenuTitle(screen));
        }

        [AvaloniaFact]
        public void Click_TheAddRowOfTheMenu_OpensTheAddPanelOfTheView()
        {
            var screen = Show();
            screen.Window.Press(PhysicalKey.W);
            var addRow = screen.Menu!.Items.OfType<MenuItem>().Single(item => Equals(item.Header, "Add"));
            addRow.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            Settle(screen.Window);

            Assert.True(screen.View.MenuAddPanel.IsOpen);
            Assert.True(screen.Menu.IsOpen);
        }

        [AvaloniaFact]
        public void Close_TheAddPanelBesideTheCursor_GivesTheFocusBackToTheCursorRow()
        {
            var screen = Show();
            var cursor = screen.Tree.CursorNode!;
            screen.Window.Press(PhysicalKey.ArrowRight, RawInputModifiers.Control);
            Assert.True(screen.View.MenuAddPanel.IsOpen);

            screen.View.MenuAddPanel.Close();
            Settle(screen.Window);

            Assert.Same(screen.RowOf(cursor), screen.Window.FocusManager!.GetFocusedElement());
        }

        [AvaloniaFact]
        public void Show_TheNewTree_HasTheToolbarOverTheRowsAndTheIdleHintsUnderThem()
        {
            var screen = Show();
            var toolbar = screen.View.GetVisualDescendants().OfType<ToolbarProjectTree>().Single();
            var rows = screen.View.Rows;

            Assert.True(toolbar.TranslatePoint(new Point(0, toolbar.Bounds.Height), screen.Window)!.Value.Y
                        <= rows.TranslatePoint(default, screen.Window)!.Value.Y);
            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
            Assert.Equal(new[] { "move", "select", "open", "menu", "add", "inspect", "undo" },
                screen.View.HintStrip.Hints!.Select(hint => hint.Words).ToArray());
            Assert.True(screen.View.HintStrip.TranslatePoint(default, screen.Window)!.Value.Y
                        >= rows.TranslatePoint(new Point(0, rows.Bounds.Height), screen.Window)!.Value.Y);
        }

        [AvaloniaFact]
        public void W_TheMenuOpens_ShowsEscCloseThenTheIdleHintsAgain()
        {
            var screen = Show();

            screen.Window.Press(PhysicalKey.W);
            Assert.Equal(TreeHintMode.Menu, screen.View.HintMode);
            Assert.Equal(new[] { "close" }, screen.View.HintStrip.Hints!.Select(hint => hint.Words).ToArray());

            screen.Menu!.Close();
            Settle(screen.Window);

            Assert.Equal(TreeHintMode.Idle, screen.View.HintMode);
            Assert.Equal("move", screen.View.HintStrip.Hints![0].Words);
        }
    }
}
