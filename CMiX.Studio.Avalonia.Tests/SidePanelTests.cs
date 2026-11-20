using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Themes;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // Each side of the window has an icon button at its top. A click opens the list of what the side can show, each with its icon
    // and its name. The user picks the tree or the tabs or the assets, on either side, or the same on both sides.
    public class SidePanelTests
    {
        private sealed class Screen
        {
            public MainWindow Window { get; init; } = null!;
            public MainViewModel ViewModel { get; init; } = null!;

            public List<PanelSide> Hosts => Window.GetVisualDescendants().OfType<PanelSide>().OrderBy(host => host.TranslatePoint(new Point(0, 0), Window)!.Value.X).ToList();
            public PanelSide Left => Hosts[0];
            public PanelSide Right => Hosts[1];
            public List<ProjectTreeView> Trees => Window.GetVisualDescendants().OfType<ProjectTreeView>().ToList();
            public List<PanelMainTabs> Tabs => Window.GetVisualDescendants().OfType<PanelMainTabs>().ToList();
            public List<AssetManager> Assets => Window.GetVisualDescendants().OfType<AssetManager>().ToList();

            public static ToggleButton ToggleOf(PanelSide host) => host.FindControl<ToggleButton>("pickerToggle")!;
            public static Popup PopupOf(PanelSide host) => host.FindControl<Popup>("pickerPopup")!;
            public static ListItemPicker ListOf(PanelSide host) => host.FindControl<ListItemPicker>("pickerList")!;

            // The rows of the list, as the user sees them after a click on the button.
            public static List<Button> RowsOf(PanelSide host) =>
                ListOf(host).GetVisualDescendants().OfType<Button>().ToList();
        }

        private static Screen Show()
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            return new Screen { Window = window, ViewModel = viewModel };
        }

        // The user opens the list and picks the row of the choice.
        private static void Pick(Screen screen, bool left, PanelChoice choice)
        {
            var host = left ? screen.Left : screen.Right;
            Screen.ToggleOf(host).IsChecked = true;
            TestServiceProviderFactory.Pump();
            var row = Screen.RowsOf(host).Single(candidate => ReferenceEquals(candidate.CommandParameter, choice));
            // A click runs the command, and then the list closes (the list posts Picked after a click).
            row.Command!.Execute(row.CommandParameter);
            row.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            TestServiceProviderFactory.Pump();
        }

        [AvaloniaFact]
        public void Show_ByDefault_PutsTheTreeLeftAndTheTabsRight()
        {
            var screen = Show();

            Assert.Single(screen.Trees);
            Assert.Single(screen.Tabs);
            Assert.Same(PanelChoice.Tree, screen.ViewModel.LeftPanel);
            Assert.Same(PanelChoice.Tabs, screen.ViewModel.RightPanel);
            Assert.True(screen.Trees[0].TranslatePoint(new Point(0, 0), screen.Window)!.Value.X < screen.Tabs[0].TranslatePoint(new Point(0, 0), screen.Window)!.Value.X);
        }

        [AvaloniaFact]
        public void Show_TheIconOfTheButton_HasTheSameSpaceOnThreeSides()
        {
            var screen = Show();

            foreach (var host in screen.Hosts)
            {
                var toggle = Screen.ToggleOf(host);
                var icon = toggle.GetVisualDescendants().OfType<AppIcon>().First(candidate => candidate.IconKey == host.Choice!.IconKey);
                var origin = icon.TranslatePoint(new Point(0, 0), toggle)!.Value;

                var left = origin.X;
                var top = origin.Y;
                var bottom = toggle.Bounds.Height - (origin.Y + icon.Bounds.Height);
                Assert.True(left > 0);
                Assert.Equal(top, left, 1);
                Assert.Equal(top, bottom, 1);

                var chevron = toggle.GetVisualDescendants().OfType<Chevron>().Single();
                var right = toggle.Bounds.Width - (chevron.TranslatePoint(new Point(chevron.Bounds.Width, 0), toggle)!.Value.X);
                Assert.Equal(left, right, 1);
            }
        }

        [AvaloniaFact]
        public void Show_EachSide_HasAnIconButtonOfTheChoiceAtItsTop()
        {
            var screen = Show();

            foreach (var host in screen.Hosts)
            {
                var toggle = Screen.ToggleOf(host);
                var controlHeight = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));
                Assert.Equal(controlHeight, toggle.Bounds.Height);
                Assert.True(toggle.Bounds.Width > controlHeight, "The button has the room for the icon and the chevron.");

                var hostTop = host.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y;
                var toggleTop = toggle.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y;
                Assert.True(toggleTop - hostTop < 10, "The button is not at the top of the panel.");

                var icon = toggle.GetVisualDescendants().OfType<AppIcon>().First(candidate => candidate.IconKey == host.Choice!.IconKey);

                // A chevron at the right of the icon tells that the button opens a list.
                var chevron = Assert.Single(toggle.GetVisualDescendants().OfType<Chevron>());
                Assert.True(chevron.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X >= icon.TranslatePoint(new Point(icon.Bounds.Width, 0), screen.Window)!.Value.X);
                Assert.Equal(180, chevron.OpenAngle);
                Assert.False(chevron.IsOpen);
                Assert.Empty(toggle.GetVisualDescendants().OfType<TextBlock>().Where(text => !string.IsNullOrEmpty(text.Text)));
            }

            Assert.Contains(Screen.ToggleOf(screen.Left).GetVisualDescendants().OfType<AppIcon>(), icon => icon.IconKey == "Tree");
            Assert.Contains(Screen.ToggleOf(screen.Right).GetVisualDescendants().OfType<AppIcon>(), icon => icon.IconKey == "Tabs");
        }

        [AvaloniaFact]
        public void Open_TheList_TurnsTheChevron()
        {
            var screen = Show();
            var toggle = Screen.ToggleOf(screen.Left);
            var chevron = Assert.Single(toggle.GetVisualDescendants().OfType<Chevron>());
            Assert.False(chevron.IsOpen);

            toggle.IsChecked = true;
            TestServiceProviderFactory.Pump();
            Assert.True(chevron.IsOpen);

            toggle.IsChecked = false;
            TestServiceProviderFactory.Pump();
            Assert.False(chevron.IsOpen);
        }

        [AvaloniaFact]
        public void Show_TheButton_HasTheNameOfTheChoiceAsTooltipBelowIt()
        {
            var screen = Show();
            var toggle = Screen.ToggleOf(screen.Left);

            Assert.Equal("Tree", ToolTip.GetTip(toggle));
            Assert.Equal(PlacementMode.Custom, ToolTip.GetPlacement(toggle));
            Assert.Equal(CMiX.UI.Popups.PopupSide.Bottom, CMiX.UI.AttachedProperties.ToolTipDefaults.GetSide(toggle));

            screen.ViewModel.LeftPanel = PanelChoice.Assets;
            TestServiceProviderFactory.Pump();
            Assert.Equal("Assets", ToolTip.GetTip(toggle));
        }

        [AvaloniaFact]
        public void Open_TheList_ShowsARowWithIconAndNameForEachChoice()
        {
            var screen = Show();
            Screen.ToggleOf(screen.Left).IsChecked = true;
            TestServiceProviderFactory.Pump();

            var rows = Screen.RowsOf(screen.Left);

            Assert.Equal(new[] { "Tree", "Tabs", "Assets" }, rows.Select(row => ((PanelChoice)row.CommandParameter!).Name));
            Assert.Equal(new[] { "Tree", "Tabs", "Resources" }, rows.Select(row => row.GetVisualDescendants().OfType<AppIcon>().Single().IconKey));
            Assert.Equal(new[] { "Tree", "Tabs", "Assets" }, rows.Select(row => row.GetVisualDescendants().OfType<TextBlock>().Single().Text));
        }

        [AvaloniaFact]
        public void Open_TheList_UsesTheSwapButtonListAndFrame()
        {
            var screen = Show();
            var popup = Screen.PopupOf(screen.Left);

            var frame = Assert.IsType<DropdownFrame>(popup.Child);
            Assert.Same(Screen.ListOf(screen.Left), frame.Content);
            Assert.Equal(PlacementMode.BottomEdgeAlignedLeft, popup.Placement);
        }

        [AvaloniaFact]
        public void Show_EveryChoice_HasAKnownIcon()
        {
            foreach (var choice in PanelChoice.All)
                Assert.True(Icons.Kinds.ContainsKey(choice.IconKey), "No icon for " + choice.Name);
        }

        [AvaloniaFact]
        public void Pick_ARow_ChangesThePanelAndClosesTheList()
        {
            var screen = Show();

            Pick(screen, left: false, PanelChoice.Tree);

            Assert.Same(PanelChoice.Tree, screen.ViewModel.RightPanel);
            Assert.False(Screen.ToggleOf(screen.Right).IsChecked);
            Assert.Equal(2, screen.Trees.Count);
        }

        [AvaloniaFact]
        public void Choose_TheTreeRightAndTheTabsLeft_ShowsThemThere()
        {
            var screen = Show();

            Pick(screen, left: true, PanelChoice.Tabs);
            Pick(screen, left: false, PanelChoice.Tree);

            Assert.Same(PanelChoice.Tabs, screen.ViewModel.LeftPanel);
            Assert.Same(PanelChoice.Tree, screen.ViewModel.RightPanel);
            var tree = Assert.Single(screen.Trees);
            var tabs = Assert.Single(screen.Tabs);
            Assert.True(tabs.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X < tree.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X);
            Assert.Same(screen.ViewModel.ProjectTree, tree.DataContext);
            Assert.Same(screen.ViewModel, tabs.DataContext);
        }

        [AvaloniaFact]
        public void Choose_TheTreeOnBothSides_KeepsOneSelection()
        {
            var screen = Show();

            Pick(screen, left: false, PanelChoice.Tree);

            Assert.Equal(2, screen.Trees.Count);
            Assert.Empty(screen.Tabs);
            Assert.All(screen.Trees, tree => Assert.Same(screen.ViewModel.ProjectTree, tree.DataContext));

            screen.ViewModel.ProjectTree.SelectedNode = screen.ViewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            TestServiceProviderFactory.Pump();

            Assert.All(screen.Trees, tree =>
            {
                var selected = tree.GetVisualDescendants().OfType<TreeViewItem>().Single(item => item.IsSelected);
                Assert.Same(screen.ViewModel.ProjectTree.SelectedNode, selected.DataContext);
            });
        }

        [AvaloniaFact]
        public void Choose_TheTabsOnEitherSide_ShowsThemThere()
        {
            var screen = Show();

            Pick(screen, left: true, PanelChoice.Tabs);

            Assert.Equal(2, screen.Tabs.Count);
            Assert.Empty(screen.Trees);
        }

        [AvaloniaFact]
        public void Choose_TheAssetsOnEitherSide_ShowsTheAssetManagerOfTheProject()
        {
            var screen = Show();

            Pick(screen, left: false, PanelChoice.Assets);

            var assets = Assert.Single(screen.Assets);
            Assert.Same(screen.ViewModel.AssetManager, assets.DataContext);
            Assert.Empty(screen.Tabs);
            Assert.Single(screen.Trees);

            Pick(screen, left: true, PanelChoice.Assets);
            Assert.Equal(2, screen.Assets.Count);
            Assert.Empty(screen.Trees);
        }

        [AvaloniaFact]
        public void Show_TheTabs_HaveNoAssetsTab()
        {
            var screen = Show();

            var keys = TestServiceProviderFactory.MainTabControl(screen.Window).Items.OfType<TabItem>()
                .Select(item => (item.Header as AppIcon)?.IconKey).ToList();

            Assert.DoesNotContain("Resources", keys);
            Assert.Empty(screen.Assets);
        }

        [AvaloniaFact]
        public void Choose_TheTabsOnEitherSide_KeepsTheMasterBeatInTheTabsPanel()
        {
            var screen = Show();
            Pick(screen, left: true, PanelChoice.Tabs);
            Pick(screen, left: false, PanelChoice.Tree);

            var beat = screen.Window.GetVisualDescendants().OfType<PanelMasterBeat>().Single();
            var tabs = Assert.Single(screen.Tabs);

            Assert.True(tabs.IsVisualAncestorOf(beat));
        }

        [AvaloniaFact]
        public void Choose_FromTheViewModelOrTheButton_KeepsBothInStep()
        {
            var screen = Show();

            screen.ViewModel.RightPanel = PanelChoice.Tree;
            TestServiceProviderFactory.Pump();
            Assert.Contains(Screen.ToggleOf(screen.Right).GetVisualDescendants().OfType<AppIcon>(), icon => icon.IconKey == "Tree");
            Assert.Equal(2, screen.Trees.Count);

            Pick(screen, left: false, PanelChoice.Tabs);
            Assert.Same(PanelChoice.Tabs, screen.ViewModel.RightPanel);
            Assert.Single(screen.Tabs);
        }

        [AvaloniaFact]
        public void Click_ATree_KeepsTheTabOnTheOtherSide()
        {
            var screen = Show();
            Pick(screen, left: true, PanelChoice.Tabs);
            Pick(screen, left: false, PanelChoice.Tree);
            var tabs = Assert.Single(screen.Tabs);
            TestServiceProviderFactory.SelectMainTab(screen.Window, "Settings");

            var tree = Assert.Single(screen.Trees);
            var layerNode = screen.ViewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            var item = tree.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, layerNode));
            var row = item.GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            var point = row.TranslatePoint(new Point(row.Bounds.Width * 0.5, row.Bounds.Height / 2), screen.Window)!.Value;
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            TestServiceProviderFactory.Pump();

            var selected = (tabs.GetVisualDescendants().OfType<TabControl>().First().SelectedItem as TabItem)?.Header as AppIcon;
            Assert.Equal("Settings", selected?.IconKey);
        }

        [AvaloniaFact]
        public void Click_ATreeWithNoTabsOnScreen_ChangesNothingElse()
        {
            var screen = Show();
            Pick(screen, left: false, PanelChoice.Tree);
            var layerNode = screen.ViewModel.ProjectTree.CompositionNodes()[0].Children[0].Children[0];
            var tree = screen.Trees[1];
            var item = tree.GetVisualDescendants().OfType<TreeViewItem>().First(row => ReferenceEquals(row.DataContext, layerNode));
            var row = item.GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            var point = row.TranslatePoint(new Point(row.Bounds.Width * 0.5, row.Bounds.Height / 2), screen.Window)!.Value;

            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            TestServiceProviderFactory.Pump();

            Assert.Same(layerNode, screen.ViewModel.ProjectTree.SelectedNode);
        }

        [AvaloniaFact]
        public void Show_TheTwoSides_HaveTheirContentAtTheSameHeight()
        {
            var screen = Show();
            var tree = Assert.Single(screen.Trees);
            var tabs = TestServiceProviderFactory.MainTabControl(screen.Window);

            Assert.Equal(tree.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y, tabs.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheHostInAnyPanel_Works()
        {
            var host = new PanelSide { Choices = PanelChoice.All, Choice = PanelChoice.Tabs };

            Assert.Same(PanelChoice.Tabs, host.Choice);
            Assert.Equal(3, host.Choices!.Cast<object>().Count());
            Assert.NotNull(host.PickCommand);
            host.PickCommand.Execute(PanelChoice.Assets);
            Assert.Same(PanelChoice.Assets, host.Choice);
        }
    }
}
