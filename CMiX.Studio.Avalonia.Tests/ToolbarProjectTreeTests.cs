using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Camera = CMiX.Core.Rendering.Cameras.Camera;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using LightEntity = CMiX.Core.Rendering.Lights.LightEntity;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The toolbar over the tree: add, duplicate, move up, move down and remove, for the selected node.
    // A button that has nothing to do for the selected node is disabled.
    public class ToolbarProjectTreeTests : IDisposable
    {
        private readonly List<Window> windows = new();

        public void Dispose()
        {
            foreach (var window in windows)
            {
                foreach (var toolbar in window.GetVisualDescendants().OfType<ToolbarProjectTree>())
                    toolbar.Panel.Close();

                foreach (var button in window.GetVisualDescendants().OfType<Button>())
                    button.ContextMenu?.Close();

                window.Close();
            }
        }

        // The toolbar on screen.
        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public ListTreeProject View { get; init; } = null!;
            public Scene Scene { get; init; } = null!;

            public Button ButtonOf(string name) => View.GetVisualDescendants().OfType<Button>().First(button => button.Name == name);

            public ToolbarProjectTree Toolbar => View.GetVisualDescendants().OfType<ToolbarProjectTree>().First();
            public AddPanel Panel => Toolbar.Panel;

            // The headers of the groups in the panel of the Add button.
            public string[] GroupHeaders() => Panel.List.Groups!.Select(group => group.Header).ToArray();

            // The box of the card of the panel, in the window.
            public Rect CardBox()
            {
                var box = Panel.Card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
                return new Rect(box.TranslatePoint(default, Window)!.Value, box.Bounds.Size);
            }

            // A click on a row of the panel.
            public void PickRow(string text)
            {
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
                // A header can have the same text as its only row, such as Layer, so the row is the first one that is not a header.
                var texts = Panel.List.VisibleTexts;
                var row = Panel.List.RowVisuals.Where((visual, index) => !visual.Classes.Contains("header") && texts[index] == text).First();
                // Every group is open, so the list can be longer than the panel: the row scrolls into view first.
                row.BringIntoView();
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                var point = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), Window)!.Value;
                Window.MouseMove(point);
                Dispatcher.UIThread.RunJobs();
                Window.MouseDown(point, MouseButton.Left);
                Window.MouseUp(point, MouseButton.Left);
                Dispatcher.UIThread.RunJobs();
            }

            public void Click(Control control) => Window.Click(control);
        }

        private Screen Show(double leftRoom = 0)
        {
            var scene = CreateScene();
            var view = new ListTreeProject { DataContext = scene.Tree, Margin = new Thickness(leftRoom, 0, 0, 0) };
            var window = new Window { Content = view, Width = 400, Height = 900 };
            window.Show();
            windows.Add(window);
            Dispatcher.UIThread.RunJobs();
            return new Screen { Window = window, View = view, Scene = scene };
        }

        [AvaloniaFact]
        public void Show_TheToolbar_IsOverTheTreeWithFiveButtons()
        {
            var screen = Show();
            var tree = screen.View.Scroller;
            var buttons = new[] { "addButton", "duplicateButton", "moveUpButton", "moveDownButton", "removeButton" }.Select(screen.ButtonOf).ToList();
            var height = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));

            Assert.All(buttons, button => Assert.Equal(height, button.Bounds.Height));
            var treeTop = tree.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y;
            Assert.All(buttons, button => Assert.True(button.TranslatePoint(new Point(0, button.Bounds.Height), screen.Window)!.Value.Y <= treeTop + 1));
        }

        [AvaloniaFact]
        public void Show_ANodeWithNothingToDo_DisablesTheButtons()
        {
            var screen = Show();
            var entityParent = screen.Scene.LayerOfA(0);
            entityParent.ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();

            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0).Children[0];
            // A composition can be added whatever is selected, so Add is never disabled.
            Assert.True(screen.ButtonOf("addButton").IsEffectivelyEnabled);
            Assert.True(screen.ButtonOf("duplicateButton").IsEffectivelyEnabled);
            Assert.False(screen.ButtonOf("moveUpButton").IsEffectivelyEnabled);

            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0);
            Assert.True(screen.ButtonOf("addButton").IsEffectivelyEnabled);
            Assert.False(screen.ButtonOf("duplicateButton").IsEffectivelyEnabled);
            Assert.False(screen.ButtonOf("removeButton").IsEffectivelyEnabled);
        }

        [AvaloniaFact]
        public void Click_AddWithACompositionSelected_OpensAPanelWithLayerAndTheProject()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Tree.CompositionNodes()[1];

            screen.Click(screen.ButtonOf("addButton"));

            Assert.True(screen.Panel.IsOpen);
            Assert.Equal(new[] { "Layer", "Filter", "Modifier", "Project" }, screen.GroupHeaders());
            Assert.Equal("Add to " + screen.Scene.B.PrefabService.Name.Value, screen.Panel.Title.Text);
            Assert.Equal(1, screen.Scene.B.LayerManager.ManagerData.Items.Count);
            Assert.Equal(2, screen.Scene.Project.CompositionManager.ManagerData.Items.Count);

            screen.PickRow("Composition");

            Assert.Equal(3, screen.Scene.Project.CompositionManager.ManagerData.Items.Count);
            Assert.Equal(1, screen.Scene.B.LayerManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void Click_TheLayerRow_AddsALayerToTheSelectedComposition()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Tree.CompositionNodes()[1];
            screen.Click(screen.ButtonOf("addButton"));

            screen.PickRow("Layer");

            Assert.Equal(2, screen.Scene.B.LayerManager.ManagerData.Items.Count);
            Assert.Equal(2, screen.Scene.Project.CompositionManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void Click_AddWithAnEntitySelected_EnablesTheEntityGroupsAndTheProject()
        {
            var screen = Show();
            screen.Scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0).Children[0];

            screen.Click(screen.ButtonOf("addButton"));

            Assert.True(screen.Panel.IsOpen);
            Assert.Equal(new[] { "Texture", "Modifier", "Project" }, screen.GroupHeaders());
            Assert.Equal(2, screen.Scene.Project.CompositionManager.ManagerData.Items.Count);
        }

        [AvaloniaFact]
        public void Click_AddWithACamerasGroupSelected_OpensAPanelThatAddsACamera()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Group<CamerasGroup>(0);

            screen.Click(screen.ButtonOf("addButton"));
            screen.PickRow("Camera");

            Assert.IsType<Camera>(Assert.Single(screen.Scene.LayerOfA(0).CameraManager.ManagerData.Items));
        }

        [AvaloniaFact]
        public void Click_AddWithALayerSelected_OpensAPanelOfContentFilterModifierAndTheProject()
        {
            var screen = Show();
            var layer = screen.Scene.LayerNodeOfA(0);
            screen.Scene.Tree.SelectedNode = layer;

            screen.Click(screen.ButtonOf("addButton"));

            Assert.True(screen.Panel.IsOpen);
            Assert.Equal(new[] { "Content", "Filter", "Modifier", "Project" }, screen.GroupHeaders());
            Assert.Equal("Add to " + layer.Header, screen.Panel.Title.Text);
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text", "Camera", "Light" }, screen.Panel.List.Groups!.Single(group => group.Header == "Content").Items.Where(item => item.IsEnabled).Select(item => item.Text));
            Assert.Empty(screen.Scene.LayerOfA(0).ModelEntityManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void AddHeader_ALayerAsTarget_NamesTheLayer()
        {
            var screen = Show();
            var layer = screen.Scene.LayerNodeOfA(0);
            screen.Scene.Tree.SelectedNode = layer;

            Assert.Equal("Add to " + layer.Header, screen.Scene.Tree.AddHeader);
        }

        [AvaloniaFact]
        public void AddHeader_NoTarget_IsNull()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = null;

            Assert.Null(screen.Scene.Tree.AddHeader);
        }

        // A click with the pointer: the middle of the search field is on the pointer, so the user can type at once.
        [AvaloniaTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void Click_Add_OpensThePanelWithItsSearchFieldUnderThePointer(bool group)
        {
            var screen = Show(leftRoom: 150);
            // Room above the toolbar, so the card can reach above the pointer.
            screen.View.Margin = new Thickness(150, 200, 0, 0);
            screen.Scene.Tree.SelectedNode = group ? screen.Scene.Group<EntitiesGroup>(0) : screen.Scene.LayerNodeOfA(0);
            var button = screen.ButtonOf("addButton");
            screen.Window.UpdateLayout();
            var pointer = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseMove(pointer);
            Dispatcher.UIThread.RunJobs();

            screen.Click(button);
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            var search = screen.Panel.List.GetVisualDescendants().OfType<TextBox>().Single(text => text.Name == "PART_Search");
            var middle = search.TranslatePoint(new Point(search.Bounds.Width / 2, search.Bounds.Height / 2), screen.Window)!.Value;
            Assert.True(screen.Panel.IsOpen);
            Assert.Equal(pointer.X, middle.X, 1.5);
            Assert.Equal(pointer.Y, middle.Y, 1.5);
        }

        // A click from the keyboard has no pointer on the button: the panel opens under the button, with its tip on it.
        [AvaloniaFact]
        public void Click_AddFromTheKeyboard_OpensThePanelEightPixelsUnderTheButtonWithTheTipOnIt()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(0);
            var button = screen.ButtonOf("addButton");
            screen.Window.MouseMove(new Point(390, 880));
            Dispatcher.UIThread.RunJobs();
            Assert.False(button.IsPointerOver);

            button.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            var origin = button.TranslatePoint(new Point(0, 0), screen.Window)!.Value;
            var box = screen.CardBox();
            Assert.Equal(origin.Y + button.Bounds.Height + 8, box.Y, 0.51);
            Assert.Equal(PopupSide.Top, screen.Panel.Card.TipEdge);
            Assert.Equal(origin.X + button.Bounds.Width / 2 - box.X, screen.Panel.Card.TipOffset, 0.51);
        }

        [AvaloniaFact]
        public void Click_APanelRow_AddsThatThingToTheSelectedLayer()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(1);
            screen.Click(screen.ButtonOf("addButton"));

            screen.PickRow("Light");

            Assert.IsType<LightEntity>(Assert.Single(screen.Scene.LayerOfA(1).LightManager.ManagerData.Items));
            Assert.Empty(screen.Scene.LayerOfA(0).LightManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Click_AddWithTheEntitiesGroupSelected_OpensAPanelOfThreeKinds()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0);

            screen.Click(screen.ButtonOf("addButton"));

            Assert.Equal(new[] { "Content", "Project" }, screen.GroupHeaders());
            Assert.Equal(new[] { "Entity", "Fullscreen Quad", "Text" }, screen.Panel.List.Groups!.Single(group => group.Header == "Content").Items.Where(item => item.IsEnabled).Select(item => item.Text));
        }

        [AvaloniaFact]
        public void Click_TheOtherButtons_ActOnTheSelectedNode()
        {
            var screen = Show();
            var first = screen.Scene.LayerOfA(0);
            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(0);

            screen.Click(screen.ButtonOf("duplicateButton"));
            Assert.Equal(3, screen.Scene.A.LayerManager.ManagerData.Items.Count);

            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(0);
            screen.Click(screen.ButtonOf("moveDownButton"));
            Assert.NotSame(first, screen.Scene.A.LayerManager.ManagerData.Items[0]);

            screen.Click(screen.ButtonOf("removeButton"));
            Assert.Equal(2, screen.Scene.A.LayerManager.ManagerData.Items.Count);
        }

        // The buttons act on the row of the arrow keys, not on the selected row (ProjectTree.TargetNode).
        [AvaloniaFact]
        public void Click_RemoveWithTheCursorOnAnotherRow_RemovesTheCursorRow()
        {
            var screen = Show();
            var first = screen.Scene.LayerOfA(0);
            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(0);
            screen.Scene.Tree.CursorNode = screen.Scene.LayerNodeOfA(1);

            screen.Click(screen.ButtonOf("removeButton"));

            Assert.Equal(new IControl[] { first }, screen.Scene.A.LayerManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void Show_EveryToolbarButton_HasATip()
        {
            var screen = Show();
            var expected = new Dictionary<string, string>
            {
                ["addButton"] = "Add",
                ["duplicateButton"] = "Duplicate",
                ["moveUpButton"] = "Move up",
                ["moveDownButton"] = "Move down",
                ["removeButton"] = "Remove"
            };

            foreach (var (name, tip) in expected)
                Assert.Equal(tip, ToolTip.GetTip(screen.ButtonOf(name))?.ToString());
        }

        [AvaloniaFact]
        public void Show_TheToolbarButtons_ShowTheShortcutsOfTheMapInTheirTips()
        {
            var screen = Show();
            var expected = new Dictionary<string, string>
            {
                ["addButton"] = "Shift+A",
                ["duplicateButton"] = "Shift+D",
                ["moveUpButton"] = "Alt+Up",
                ["moveDownButton"] = "Alt+Down",
                ["removeButton"] = "Delete"
            };

            foreach (var (name, shortcut) in expected)
                Assert.Equal(shortcut, Assert.IsType<CMiX.UI.Popups.TipContent>(ToolTip.GetTip(screen.ButtonOf(name))).Shortcut);
        }

        [AvaloniaFact]
        public void Set_AnotherGestureInTheMap_ChangesTheShortcutInTheTip()
        {
            var screen = Show();

            screen.Scene.Tree.Shortcuts.Set("Tree.Add", new KeyGesture(Key.N, KeyModifiers.Control));

            Assert.Equal("Ctrl+N", Assert.IsType<CMiX.UI.Popups.TipContent>(ToolTip.GetTip(screen.ButtonOf("addButton"))).Shortcut);
        }

        [AvaloniaFact]
        public void Select_NothingToDuplicate_ShowsWhyInTheTipOfTheDisabledButton()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0);
            Dispatcher.UIThread.RunJobs();

            var button = screen.ButtonOf("duplicateButton");
            var tip = Assert.IsType<CMiX.UI.Popups.TipContent>(ToolTip.GetTip(button));

            Assert.False(button.IsEffectivelyEnabled);
            Assert.Equal("Select a row to duplicate.", tip.Reason);
            Assert.True(ToolTip.GetShowOnDisabled(button));
        }

        [AvaloniaFact]
        public void Select_ARowToDuplicate_ClearsTheReasonFromTheTip()
        {
            var screen = Show();
            screen.Scene.Tree.SelectedNode = screen.Scene.Group<EntitiesGroup>(0);
            Dispatcher.UIThread.RunJobs();

            screen.Scene.Tree.SelectedNode = screen.Scene.LayerNodeOfA(0);
            Dispatcher.UIThread.RunJobs();

            Assert.Null(Assert.IsType<CMiX.UI.Popups.TipContent>(ToolTip.GetTip(screen.ButtonOf("duplicateButton"))).Reason);
        }
    }
}
