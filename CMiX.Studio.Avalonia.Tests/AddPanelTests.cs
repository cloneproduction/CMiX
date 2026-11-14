using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Rendering.Cameras;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Popups;
using Xunit;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The Add panel: the groups it shows, where it opens next to an anchor, where its search field and its tip are, and what a pick does.
    public class AddPanelTests : IDisposable
    {
        private sealed record Setup(Window Window, Button Anchor, AddPanel Panel, Scene Scene);

        private readonly List<Setup> shown = new();

        // A panel and its window are closed after each test, so one test does not leave a popup open for the next.
        public void Dispose()
        {
            foreach (var setup in shown)
            {
                setup.Panel.Close();
                setup.Window.Close();
            }

            Dispatcher.UIThread.RunJobs();
        }

        private Setup Show(double left = 40, double top = 100)
        {
            var scene = TreeFixtures.CreateScene([2]);
            var anchor = new Button
            {
                Content = "+", Width = 80, Height = 24,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(left, top, 0, 0),
            };
            var window = new Window { Content = new Panel { Children = { anchor } }, Width = 900, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var setup = new Setup(window, anchor, new AddPanel(), scene);
            shown.Add(setup);
            return setup;
        }

        private static void Open(Setup setup, PopupSide side, string? header = null, TreeNode? node = null)
        {
            setup.Panel.Open(setup.Anchor, side, setup.Scene.Tree.GetAddGroups(node ?? setup.Scene.LayerNode), header);
            Dispatcher.UIThread.RunJobs();
            setup.Window.UpdateLayout();
        }

        private static Rect BoxOf(Setup setup)
        {
            var box = setup.Panel.Card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
            return new Rect(box.TranslatePoint(default, setup.Window)!.Value, box.Bounds.Size);
        }

        private static Rect SearchOf(Setup setup)
        {
            var search = setup.Panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");
            return new Rect(search.TranslatePoint(default, setup.Window)!.Value, search.Bounds.Size);
        }

        private static void Click(Setup setup, Control control)
        {
            setup.Window.UpdateLayout();
            var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), setup.Window)!.Value;
            setup.Window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
            setup.Window.MouseDown(point, MouseButton.Left);
            setup.Window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void OpenAtPoint_APoint_PutsTheMiddleOfTheSearchFieldUnderIt()
        {
            var setup = Show();
            var point = new Point(300, 250);

            setup.Panel.OpenAtPoint(setup.Window, point, setup.Scene.Tree.GetAddGroups(setup.Scene.LayerNode), "Add");
            Dispatcher.UIThread.RunJobs();
            setup.Window.UpdateLayout();

            var search = SearchOf(setup);
            Assert.True(setup.Panel.IsOpen);
            Assert.InRange(point.X, search.Left, search.Right);
            Assert.Equal(point.Y, search.Center.Y, 1.5);
            Assert.Equal(point.X, search.Center.X, 1.5);
            Assert.Equal("Add", setup.Panel.Title.Text);
        }

        [AvaloniaFact]
        public void Open_AnyNode_OpensEveryGroupEvenALongOne()
        {
            var setup = Show();

            Open(setup, PopupSide.Right);

            var groups = setup.Panel.List.Groups!.ToList();
            Assert.Contains(groups, group => group.Items.Count > 8);
            Assert.All(groups, group => Assert.True(setup.Panel.List.IsOpen(group)));
        }

        [AvaloniaFact]
        public void Open_ALayerNode_ShowsItsGroups()
        {
            var setup = Show();

            Open(setup, PopupSide.Right);

            Assert.True(setup.Panel.IsOpen);
            var texts = setup.Panel.List.VisibleTexts;
            Assert.Equal(new[] { "Content", "Filter", "Modifier", "Project" }, setup.Panel.List.Groups!.Select(group => group.Header));
            Assert.Equal(new[] { "Content", "Entity", "Fullscreen Quad", "Text", "Camera", "Light" }, texts.Skip(texts.ToList().IndexOf("Content")).Take(6));
        }

        [AvaloniaFact]
        public void Open_ToTheRight_PutsTheCardAtTheGapAndTheSearchFieldOnTheAnchor()
        {
            var setup = Show();

            Open(setup, PopupSide.Right);

            var box = BoxOf(setup);
            var search = SearchOf(setup);
            Assert.Equal(40 + 80 + 8, box.X, 0.51);
            Assert.Equal(100 + 12, search.Center.Y, 0.51);
            Assert.Equal(PopupSide.Left, setup.Panel.Card.TipEdge);
            Assert.True(setup.Panel.Card.ShowsTip);
            Assert.Equal(112 - box.Y, setup.Panel.Card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_ToTheRightWithAHeader_KeepsTheSearchFieldOnTheAnchor()
        {
            var setup = Show();

            Open(setup, PopupSide.Right, "Add to Layer 02");

            Assert.True(setup.Panel.Title.IsVisible);
            Assert.Equal("Add to Layer 02", setup.Panel.Title.Text);
            Assert.Equal(112, SearchOf(setup).Center.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_WithoutAHeader_HidesTheHeaderLine()
        {
            var setup = Show();

            Open(setup, PopupSide.Right, "Add to Layer 02");
            Open(setup, PopupSide.Right);

            Assert.False(setup.Panel.Title.IsVisible);
            Assert.Equal(112, SearchOf(setup).Center.Y, 0.51);
        }

        [AvaloniaFact]
        public void Open_Below_PutsTheCardAtTheGapAndTheTipOnTheMiddleOfTheAnchor()
        {
            var setup = Show();

            Open(setup, PopupSide.Bottom);

            var box = BoxOf(setup);
            Assert.Equal(100 + 24 + 8, box.Y, 0.51);
            Assert.Equal(PopupSide.Top, setup.Panel.Card.TipEdge);
            Assert.Equal(80 - box.X, setup.Panel.Card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Open_NoRoomAtTheRight_FlipsToTheLeft()
        {
            var setup = Show(left: 500);
            setup.Panel.Anchoring.Bounds = new Rect(0, 0, 640, 600);

            Open(setup, PopupSide.Right);

            var box = BoxOf(setup);
            Assert.Equal(PopupSide.Left, setup.Panel.Anchoring.Last!.Value.Side);
            Assert.Equal(PopupSide.Right, setup.Panel.Card.TipEdge);
            Assert.Equal(500 - 8, box.Right, 0.51);
        }

        [AvaloniaFact]
        public void Open_NoGroups_OpensNothing()
        {
            var setup = Show();

            setup.Panel.Open(setup.Anchor, PopupSide.Right, Array.Empty<TreeAddGroup>());
            Dispatcher.UIThread.RunJobs();

            Assert.False(setup.Panel.IsOpen);
        }

        [AvaloniaFact]
        public void Open_ForAnotherNode_ShowsTheGroupsOfThatNode()
        {
            var setup = Show();

            Open(setup, PopupSide.Right);
            Open(setup, PopupSide.Right, node: setup.Scene.CompositionNode);

            Assert.Equal(new[] { "Layer", "Layer" }, setup.Panel.List.VisibleTexts.Take(2));
            Assert.True(setup.Panel.IsOpen);
        }

        [AvaloniaFact]
        public void Open_TheRows_HaveASquareInTheColorOfTheirKind()
        {
            
            var setup = Show();

            Open(setup, PopupSide.Right);

            var texts = setup.Panel.List.VisibleTexts;
            var row = setup.Panel.List.RowVisuals.Where((visual, index) => !visual.Classes.Contains("header") && texts[index] == "Entity").First();
            var square = ((StackPanel)row.Child!).Children.OfType<Border>().Single();
            var entity = CMiX.Studio.Avalonia.Themes.CategoryPalette.FromApplication().For(CMiX.Studio.Avalonia.ViewModels.Tree.TreeCategory.Entity).Kind;
            Assert.Empty(((StackPanel)row.Child!).Children.OfType<CMiX.UI.Controls.AppIcon>());
            Assert.Equal(entity, ((ISolidColorBrush)square.Background!).Color);
        }

        [AvaloniaFact]
        public void Pick_ARow_ClosesThePanelThenAddsTheItem()
        {
            var setup = Show();
            Open(setup, PopupSide.Right);
            var index = setup.Panel.List.VisibleTexts.ToList().IndexOf("Camera");

            Click(setup, setup.Panel.List.RowVisuals[index]);
            Dispatcher.UIThread.RunJobs();

            Assert.False(setup.Panel.IsOpen);
            Assert.IsType<Camera>(Assert.Single(setup.Scene.Layer.CameraManager.ManagerData.Items));
        }

        [AvaloniaFact]
        public void Press_EscapeInTheSearchField_ClosesThePanelAndAddsNothing()
        {
            var setup = Show();
            Open(setup, PopupSide.Right);
            var search = setup.Panel.List.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");

            search.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
            Dispatcher.UIThread.RunJobs();

            Assert.False(setup.Panel.IsOpen);
            Assert.Empty(setup.Scene.Layer.CameraManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Close_ThePanel_RaisesClosed()
        {
            var setup = Show();
            var closed = 0;
            setup.Panel.Closed += (_, _) => closed++;
            Open(setup, PopupSide.Right);

            setup.Panel.Close();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, closed);
            Assert.False(setup.Panel.IsOpen);
        }
    }
}
