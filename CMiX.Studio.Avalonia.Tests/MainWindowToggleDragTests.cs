using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // A drag over the visibility or the mask buttons of the real window sets them all to one state, as one undo step.
    public class MainWindowToggleDragTests
    {
        private sealed class Screen
        {
            public MainWindow Window { get; init; } = null!;
            public MainViewModel ViewModel { get; init; } = null!;
            public UndoManager Undo { get; init; } = null!;
            public TreeViewProject TreeView { get; init; } = null!;
            public List<Layer> Layers { get; } = new();

            public TreeNode LayerNode(int index) => ViewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[index];

            private TreeViewItem ItemOf(TreeNode node) =>
                TreeView.GetVisualDescendants().OfType<TreeViewItem>().First(item => ReferenceEquals(item.DataContext, node));

            // The buttons in the toggle column of a node, not the buttons in the rows below it.
            public ToggleButton ButtonOf(TreeNode node, string group) =>
                ItemOf(node).GetVisualDescendants().OfType<TogglesRow>().First()
                    .GetVisualDescendants().OfType<ToggleButton>().First(button => ToggleDrag.GetGroup(button) == group);

            public Point CenterOf(ToggleButton button)
            {
                Window.UpdateLayout();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
                return button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), Window)!.Value;
            }

            public void Down(ToggleButton button) => Window.MouseDown(CenterOf(button), MouseButton.Left);

            public void MoveTo(ToggleButton button) => Window.MouseMove(CenterOf(button), RawInputModifiers.LeftMouseButton);

            public void Up(ToggleButton button) => Window.MouseUp(CenterOf(button), MouseButton.Left);

            // The visibility buttons that are not in the tree: the ones of the lists in the tabs.
            public List<ToggleButton> ListButtons() =>
                Window.GetVisualDescendants().OfType<ToggleButton>()
                    .Where(button => ToggleDrag.GetGroup(button) == "visibility" && button.IsEffectivelyVisible && button.Bounds.Width > 0
                                     && !TreeView.IsVisualAncestorOf(button))
                    .ToList();
        }

        // One composition with four layers.
        private static Screen Show()
        {
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            var screen = new Screen
            {
                Window = window,
                ViewModel = viewModel,
                Undo = provider.GetRequiredService<UndoManager>(),
                TreeView = window.GetVisualDescendants().OfType<TreeViewProject>().Single()
            };
            for (var i = 0; i < 4; i++)
            {
                composition.LayerManager.AddItem(typeof(Layer));
                screen.Layers.Add((Layer)composition.LayerManager.SelectedItem);
            }

            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            screen.CenterOf(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            return screen;
        }

        [AvaloniaFact]
        public void Show_TheMainWindow_HasTheToggleDragBehavior()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();

            Assert.Single(global::Avalonia.Xaml.Interactivity.Interaction.GetBehaviors(window).OfType<ToggleDragBehavior>());
        }

        [AvaloniaFact]
        public void Show_TheVisibilityAndMaskButtons_AreInTheirGroups()
        {
            var screen = Show();

            Assert.Equal("visibility", ToggleDrag.GetGroup(screen.ButtonOf(screen.LayerNode(0), "visibility")));
            Assert.Equal("mask", ToggleDrag.GetGroup(screen.ButtonOf(screen.LayerNode(0), "mask")));
        }

        [AvaloniaFact]
        public void Drag_DownTheTree_SetsTheVisibilityOfEveryLayerPassed()
        {
            var screen = Show();
            var start = screen.Layers[0].PrefabService.Visibility.Value;

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            for (var i = 1; i < 4; i++)
                screen.MoveTo(screen.ButtonOf(screen.LayerNode(i), "visibility"));
            screen.Up(screen.ButtonOf(screen.LayerNode(3), "visibility"));

            Assert.All(screen.Layers, layer => Assert.Equal(!start, layer.PrefabService.Visibility.Value));
        }

        [AvaloniaFact]
        public void Drag_FastDownTheTree_SkipsNoLayer()
        {
            var screen = Show();
            var start = screen.Layers[0].PrefabService.Visibility.Value;

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            screen.MoveTo(screen.ButtonOf(screen.LayerNode(3), "visibility"));
            screen.Up(screen.ButtonOf(screen.LayerNode(3), "visibility"));

            Assert.All(screen.Layers, layer => Assert.Equal(!start, layer.PrefabService.Visibility.Value));
        }

        [AvaloniaFact]
        public void Drag_OnTheVisibility_KeepsTheMask()
        {
            var screen = Show();
            var masks = screen.Layers.Select(layer => layer.Mask.IsMask.Value).ToList();

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            screen.MoveTo(screen.ButtonOf(screen.LayerNode(3), "mask"));
            screen.Up(screen.ButtonOf(screen.LayerNode(3), "mask"));

            Assert.Equal(masks, screen.Layers.Select(layer => layer.Mask.IsMask.Value).ToList());
        }

        [AvaloniaFact]
        public void Drag_OnTheMaskDownTheTree_SetsOnlyTheMaskOfEveryLayerPassed()
        {
            var screen = Show();
            var start = screen.Layers[0].Mask.IsMask.Value;
            var visibilities = screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList();

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "mask"));
            screen.MoveTo(screen.ButtonOf(screen.LayerNode(3), "mask"));
            screen.Up(screen.ButtonOf(screen.LayerNode(3), "mask"));

            Assert.All(screen.Layers, layer => Assert.Equal(!start, layer.Mask.IsMask.Value));
            Assert.Equal(visibilities, screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList());
        }

        [AvaloniaFact]
        public void Drag_OverManyLayers_IsOneUndoStep()
        {
            var screen = Show();
            var before = screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList();
            screen.Undo.Clear();

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            screen.MoveTo(screen.ButtonOf(screen.LayerNode(3), "visibility"));
            screen.Up(screen.ButtonOf(screen.LayerNode(3), "visibility"));
            Assert.NotEqual(before, screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList());

            screen.Undo.Undo();

            Assert.Equal(before, screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList());
            Assert.False(screen.Undo.CanUndo);
        }

        [AvaloniaFact]
        public void BeginUndoCaptureCommand_ThenTwoChanges_AreOneUndoStep()
        {
            var screen = Show();
            var before = screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList();
            screen.Undo.Clear();

            screen.ViewModel.BeginUndoCaptureCommand.Execute(null);
            screen.Layers[0].PrefabService.Visibility.Value = !before[0];
            screen.Layers[1].PrefabService.Visibility.Value = !before[1];
            screen.ViewModel.EndUndoCaptureCommand.Execute(null);
            screen.Undo.Undo();

            Assert.Equal(before, screen.Layers.Select(layer => layer.PrefabService.Visibility.Value).ToList());
            Assert.False(screen.Undo.CanUndo);
        }

        [AvaloniaFact]
        public void Click_AToggle_TogglesOnceInOneUndoStep()
        {
            var screen = Show();
            var before = screen.Layers[1].PrefabService.Visibility.Value;
            screen.Undo.Clear();

            screen.Down(screen.ButtonOf(screen.LayerNode(1), "visibility"));
            screen.Up(screen.ButtonOf(screen.LayerNode(1), "visibility"));
            Assert.Equal(!before, screen.Layers[1].PrefabService.Visibility.Value);

            screen.Undo.Undo();

            Assert.Equal(before, screen.Layers[1].PrefabService.Visibility.Value);
        }

        [AvaloniaFact]
        public void Drag_FromTheTreeToAList_PaintsTheButtonsOfTheList()
        {
            var screen = Show();

            // The Composition tab shows the list of the compositions. A second composition gives a second button in it.
            screen.ViewModel.Project.CompositionManager.AddItem(typeof(Composition));
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Composition");
            TestServiceProviderFactory.Pump();
            var listButtons = screen.ListButtons();
            Assert.True(listButtons.Count >= 2, "The test needs at least two visibility buttons in the lists.");
            var start = screen.Layers[0].PrefabService.Visibility.Value;

            screen.Down(screen.ButtonOf(screen.LayerNode(0), "visibility"));
            foreach (var button in listButtons)
                screen.MoveTo(button);
            screen.Up(listButtons[^1]);

            Assert.All(listButtons, button => Assert.Equal(!start, button.IsChecked));
        }

        [AvaloniaFact]
        public void Press_AVisibilityButton_EndsTheRenameOfARow()
        {
            var screen = Show();
            var box = screen.TreeView.GetVisualDescendants().OfType<TextBoxEditable>().First(row => row.Text == screen.LayerNode(0).Header);
            var input = box.GetVisualDescendants().OfType<TextBox>().First();
            box.IsEditing = true;
            TestServiceProviderFactory.Pump();
            Assert.True(input.IsVisible);

            screen.Down(screen.ButtonOf(screen.LayerNode(1), "visibility"));
            screen.Up(screen.ButtonOf(screen.LayerNode(1), "visibility"));

            Assert.False(input.IsVisible);
        }
    }
}
