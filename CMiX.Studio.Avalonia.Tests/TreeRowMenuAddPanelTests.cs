using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Entity = CMiX.Core.Compositing.Entity;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Add row of the menu of a tree row: it opens the Add panel beside the menu, with the search field on the row, and the menu and
    // the panel close together.
    public class TreeRowMenuAddPanelTests : IDisposable
    {
        private readonly List<(Window Window, TreeViewProject View)> shown = new();

        public void Dispose()
        {
            foreach (var (window, view) in shown)
            {
                view.TreeView().ContextMenu?.Close();
                view.MenuAddPanel.Close();
                window.Close();
            }

            Dispatcher.UIThread.RunJobs();
        }

        private sealed record Screen(Window Window, TreeViewProject View, Scene Scene)
        {
            public TreeView TreeView => View.TreeView();

            public ContextMenu Menu => TreeView.ContextMenu!;

            public AddPanel Panel => View.MenuAddPanel;

            public TreeViewItem RowOf(TreeNode node) =>
                View.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node));

            public MenuItem ItemOf(string header) => Menu.Items.OfType<MenuItem>().Single(item => Equals(item.Header, header));

            public void Settle()
            {
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
            }

            // A right click on the row. A request with no place of the pointer comes from a key and opens the menu under the row.
            public ContextMenu OpenMenu(TreeNode node)
            {
                var point = CenterOf(TreeTestScreen.RowOf(View, node));
                Window.MouseMove(point);
                Window.MouseDown(point, MouseButton.Right);
                Window.MouseUp(point, MouseButton.Right);
                Settle();
                return Menu;
            }

            public void Click(string header)
            {
                ItemOf(header).RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                Settle();
            }

            public Point CenterOf(Control control) =>
                control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), Window)!.Value;

            public Rect CardBox()
            {
                var box = Panel.Card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
                return new Rect(box.TranslatePoint(default, Window)!.Value, box.Bounds.Size);
            }

            public Rect MenuBox()
            {
                var frame = Menu.GetVisualDescendants().OfType<PopupFrame>().First();
                var origin = frame.TranslatePoint(default, Window)!.Value;
                return new Rect(origin.X + PopupFrame.ShadowMargin.Left, origin.Y + PopupFrame.ShadowMargin.Top,
                    frame.Bounds.Width - PopupFrame.ShadowMargin.Left - PopupFrame.ShadowMargin.Right,
                    frame.Bounds.Height - PopupFrame.ShadowMargin.Top - PopupFrame.ShadowMargin.Bottom);
            }
        }

        // A composition with two layers. The first layer holds an entity.
        private Screen Show()
        {
            var scene = TreeFixtures.CreateScene([2]);
            scene.Layer.ModelEntityManager.AddItem(typeof(Entity));
            var view = new TreeViewProject { DataContext = scene.Tree, Width = 400, HorizontalAlignment = HorizontalAlignment.Left };
            var window = new Window { Content = view, Width = 1000, Height = 700 };
            window.Show();
            shown.Add((window, view));
            var screen = new Screen(window, view, scene);
            screen.Settle();
            return screen;
        }

        [AvaloniaFact]
        public void Click_TheAddRow_OpensThePanelWithTheGroupsOfTheRowAndKeepsTheMenuOpen()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);

            screen.Click("Add");

            Assert.True(screen.Panel.IsOpen);
            Assert.True(screen.Menu.IsOpen);
            Assert.Equal(new[] { "Content", "Filter", "Modifier", "Project" }, screen.Panel.List.Groups!.Select(group => group.Header));
        }

        [AvaloniaFact]
        public void Click_TheAddRow_PutsTheSearchFieldOnTheRowAndThePanelAtTheGapFromTheMenuCard()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);

            screen.Click("Add");

            var row = screen.ItemOf("Add");
            var search = screen.Panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");
            var searchCenter = search.TranslatePoint(new Point(0, search.Bounds.Height / 2), screen.Window)!.Value.Y;
            Assert.Equal(screen.CenterOf(row).Y, searchCenter, 0.51);
            Assert.Equal(screen.MenuBox().Right + 8, screen.CardBox().X, 0.51);
            Assert.Equal(PopupSide.Left, screen.Panel.Card.TipEdge);
        }

        [AvaloniaFact]
        public void Click_TheAddRow_MakesThePanelACompanionOfTheMenu()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);

            screen.Click("Add");

            Assert.Same(screen.Panel.Card, ContextMenuPointerAway.GetCompanion(screen.Menu));
        }

        [AvaloniaFact]
        public void Move_ThePointerOverThePanel_KeepsTheMenuOpen()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);
            screen.Click("Add");

            var box = screen.CardBox();
            screen.Window.MouseMove(new Point(box.Center.X, box.Center.Y));
            Dispatcher.UIThread.RunJobs();

            Assert.True(screen.Menu.IsOpen);
            Assert.True(screen.Panel.IsOpen);
        }

        [AvaloniaFact]
        public void Move_ThePointerOntoAnotherRow_ClosesThePanelOnly()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);
            screen.Click("Add");

            var other = screen.ItemOf("Duplicate");
            screen.Window.MouseMove(screen.CenterOf(other));
            Dispatcher.UIThread.RunJobs();

            Assert.False(screen.Panel.IsOpen);
            Assert.True(screen.Menu.IsOpen);
        }

        [AvaloniaFact]
        public void Pick_ARowOfThePanel_ClosesThePanelAndTheMenuThenAdds()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNodeOfA(1));
            var menu = screen.Menu;
            screen.Click("Add");

            var texts = screen.Panel.List.VisibleTexts;
            var row = screen.Panel.List.RowVisuals.Where((visual, index) => !visual.Classes.Contains("header") && texts[index] == "Camera").First();
            var point = screen.CenterOf(row);
            screen.Window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.False(screen.Panel.IsOpen);
            Assert.False(menu.IsOpen);
            Assert.Single(screen.Scene.LayerOfA(1).CameraManager.ManagerData.Items);
            Assert.Empty(screen.Scene.LayerOfA(0).CameraManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Press_EscapeInThePanel_ClosesThePanelAndLeavesTheMenu()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);
            screen.Click("Add");
            var search = screen.Panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");

            search.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
            Dispatcher.UIThread.RunJobs();

            Assert.False(screen.Panel.IsOpen);
            Assert.True(screen.Menu.IsOpen);
        }

        [AvaloniaFact]
        public void Close_TheMenu_ClosesThePanelAndDropsTheCompanion()
        {
            var screen = Show();
            var menu = screen.OpenMenu(screen.Scene.LayerNode);
            screen.Click("Add");

            menu.Close();
            Dispatcher.UIThread.RunJobs();

            Assert.False(screen.Panel.IsOpen);
            Assert.Null(ContextMenuPointerAway.GetCompanion(menu));
        }

        [AvaloniaFact]
        public void Press_RightOnTheAddRow_OpensThePanel()
        {
            var screen = Show();
            screen.OpenMenu(screen.Scene.LayerNode);

            screen.ItemOf("Add").RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Right });
            screen.Settle();

            Assert.True(screen.Panel.IsOpen);
        }

        [AvaloniaFact]
        public void Open_TheMenuOfAnEntity_OpensOnAddLikeTheOtherRows()
        {
            var screen = Show();
            var entity = Scene.Group<EntitiesGroup>(screen.Scene.LayerNode).Children[0];

            screen.OpenMenu(entity);

            Assert.True(MenuAnchor.GetIsAnchor(screen.ItemOf("Add")));
            Assert.False(MenuAnchor.GetIsAnchor(screen.ItemOf("Duplicate")));
        }

        [AvaloniaFact]
        public void Click_TheAddRowOfAnEntity_ShowsOnlyTextureModifierAndTheProject()
        {
            var screen = Show();
            var entity = Scene.Group<EntitiesGroup>(screen.Scene.LayerNode).Children[0];
            screen.OpenMenu(entity);

            screen.Click("Add");

            Assert.Equal(new[] { "Texture", "Modifier", "Project" }, screen.Panel.List.Groups!.Select(group => group.Header));
        }
    }

    internal static class TreeViewProjectExtensions
    {
        public static TreeView TreeView(this TreeViewProject view) => view.GetVisualDescendants().OfType<TreeView>().First();
    }
}
