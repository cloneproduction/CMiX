using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Controls.String;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Composition = CMiX.Core.Compositing.Composition;
using Entity = CMiX.Core.Compositing.Entity;
using Layer = CMiX.Core.Compositing.Layer;
using Project = CMiX.Core.Compositing.Project;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree of the project on screen: one row per node, and a drag that follows the rules of TreeDropService.
    public class TreeViewProjectTests
    {
        private sealed class Screen
        {
            public Window Window { get; init; } = null!;
            public TreeViewProject View { get; init; } = null!;
            public ProjectTree Tree { get; init; } = null!;
            public Project Project { get; init; } = null!;
            public Composition A { get; init; } = null!;
            public Composition B { get; init; } = null!;

            public TreeView TreeView => View.GetVisualDescendants().OfType<TreeView>().First();

            public TreeViewItem RowOf(TreeNode node) => TreeTestScreen.ItemOf(View, node);

            // A point in the header of a row, as a fraction of its height.
            public Point PointIn(TreeNode node, double fraction)
            {
                var header = RowOf(node).GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");
                return header.TranslatePoint(new Point(header.Bounds.Width / 2, header.Bounds.Height * fraction), Window)!.Value;
            }

            // Drags the source row over the target and keeps the button down.
            public void DragOver(TreeNode source, TreeNode target, double fraction)
            {
                TreeDrag.Render(Window);
                TreeDrag.Over(Window, PointIn(source, 0.5), PointIn(target, fraction));
            }

            // Drags the source row to the target and drops it there.
            public void Drop(TreeNode source, TreeNode target, double fraction)
            {
                TreeDrag.Render(Window);
                TreeDrag.Drop(Window, PointIn(source, 0.5), PointIn(target, fraction));
            }
        }

        // A holds two layers, B holds one.
        private static Screen Show()
        {
            var scene = CreateScene();
            var screen = TreeTestScreen.Show(scene.Tree, 400, 1100, instant: false, settle: false);
            Dispatcher.UIThread.RunJobs();
            return new Screen { Window = screen.Window, View = screen.View, Tree = scene.Tree, Project = scene.Project, A = scene.A, B = scene.B };
        }

        private static int CountNodes(TreeNode node) => 1 + node.Children.Sum(CountNodes);

        [AvaloniaFact]
        public void Show_TheView_ShowsOneRowPerNode()
        {
            var screen = Show();

            var rows = screen.View.GetVisualDescendants().OfType<TreeViewItem>().Count();

            Assert.Equal(screen.Tree.Roots.Sum(CountNodes), rows);
        }

        private static Border HeaderOf(TreeViewItem row) =>
            row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "RowBorder");

        // The toggles of a row: the ones in its toggle column, from the left. The column of the first row found is its own.
        private static List<ToggleButton> TogglesOf(TreeViewItem row) =>
            row.GetVisualDescendants().OfType<TogglesRow>().First()
                .GetVisualDescendants().OfType<ToggleButton>()
                .Where(toggle => toggle.IsEffectivelyVisible).ToList();


        [AvaloniaFact]
        public void Show_EveryRow_HasTheRowHeightOfTheOtherLists()
        {
            var screen = Show();
            screen.A.LayerManager.ManagerData.Items.OfType<Layer>().First().ModelEntityManager.AddItem(typeof(Entity));
            TestServiceProviderFactory.ExpandAll(screen.Tree);
            Dispatcher.UIThread.RunJobs();
            var expected = Convert.ToDouble(Application.Current!.FindResource("ControlHeightMD"));

            var heights = TreeTestScreen.All(screen.Tree.Roots).Select(node => HeaderOf(screen.RowOf(node)).Bounds.Height).ToList();

            Assert.NotEmpty(heights);
            Assert.All(heights, height => Assert.Equal(expected, height));
        }

        [AvaloniaTheory]
        [InlineData(0.1)]
        [InlineData(0.5)]
        [InlineData(0.9)]
        public void DragOver_ARow_MovesNoRow(double fraction)
        {
            var screen = Show();
            var nodes = TreeTestScreen.All(screen.Tree.Roots).ToList();
            var before = nodes.Select(node => HeaderOf(screen.RowOf(node)).TranslatePoint(new Point(0, 0), screen.Window)).ToList();
            var heightsBefore = nodes.Select(node => HeaderOf(screen.RowOf(node)).Bounds.Height).ToList();

            screen.DragOver(screen.Tree.CompositionNodes()[0].Children[0].Children[0], screen.Tree.CompositionNodes()[1], fraction);

            Assert.Equal(before, nodes.Select(node => HeaderOf(screen.RowOf(node)).TranslatePoint(new Point(0, 0), screen.Window)));
            Assert.Equal(heightsBefore, nodes.Select(node => HeaderOf(screen.RowOf(node)).Bounds.Height));
        }

        [AvaloniaFact]
        public void Show_TwoRowsNextToEachOther_AreOneRowGapApart()
        {
            var screen = Show();
            var spacing = Convert.ToDouble(screen.View.Resources["TreeRowGap"]);
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var firstGroup = HeaderOf(screen.RowOf(layerNode.Children[0]));
            var secondGroup = HeaderOf(screen.RowOf(layerNode.Children[1]));

            var gap = secondGroup.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y
                - (firstGroup.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y + firstGroup.Bounds.Height);

            Assert.Equal(3, spacing);
            Assert.Equal(spacing, gap);
        }

        [AvaloniaFact]
        public void Show_AChildRow_IsOneRowGapBelowItsParent()
        {
            var screen = Show();
            var spacing = Convert.ToDouble(screen.View.Resources["TreeRowGap"]);
            var parent = HeaderOf(screen.RowOf(screen.Tree.CompositionNodes()[0]));
            var child = HeaderOf(screen.RowOf(screen.Tree.CompositionNodes()[0].Children[0]));

            var gap = child.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y
                - (parent.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y + parent.Bounds.Height);

            Assert.Equal(spacing, gap);
        }

        private static TreeNode LastVisibleOf(TreeNode node) =>
            node.IsExpanded && node.Children.Count > 0 ? LastVisibleOf(node.Children[^1]) : node;

        // The own mark line of an item: the last one in the tree order, because the rows under the item come before it.
        private static Border DropLineOf(TreeViewItem row) =>
            row.GetVisualDescendants().OfType<Border>().Last(border => border.Name == "DropLine");

        // The place of the drop that the marker keeps for the node. It is null when the marker does not mark the node.
        private static DropPosition? MarkOf(Screen screen, TreeNode node) =>
            ReferenceEquals(screen.View.Marker.Marked?.DataContext, node) ? screen.View.Marker.MarkedPosition : null;

        // True when the marker marks a drop before or after the node.
        private static bool IsMarkedBetween(Screen screen, TreeNode node) =>
            MarkOf(screen, node) is DropPosition.Before or DropPosition.After;

        private static double Top(Screen screen, Control control) => control.TranslatePoint(new Point(0, 0), screen.Window)!.Value.Y;

        // Three compositions. The last one holds a layer, so its block is the composition, the layer and the three groups of the layer.
        private static Screen ShowWithABlock()
        {
            var screen = Show();
            screen.Project.CompositionManager.AddItem(typeof(Composition));
            ((Composition)screen.Project.CompositionManager.ManagerData.Items[2]).LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.ExpandAll(screen.Tree);
            Dispatcher.UIThread.RunJobs();
            screen.Window.UpdateLayout();
            return screen;
        }

        [AvaloniaTheory]
        [InlineData(0.05)]
        [InlineData(0.4)]
        public void DragOver_ACompositionOnTheTopHalfOfACompositionRow_MarksTheDropBefore(double fraction)
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];

            screen.DragOver(screen.Tree.CompositionNodes()[0], last, fraction);

            Assert.Equal(DropPosition.Before, MarkOf(screen, last));
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(screen.RowOf(LastVisibleOf(screen.Tree.CompositionNodes()[1]))));
        }

        [AvaloniaTheory]
        [InlineData(0.6)]
        [InlineData(0.95)]
        public void DragOver_ACompositionOnTheBottomHalfOfTheLastRowOfABlock_MarksTheDropAfterTheBlock(double fraction)
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];
            var lastRow = LastVisibleOf(last);
            Assert.NotSame(last, lastRow);

            screen.DragOver(screen.Tree.CompositionNodes()[0], lastRow, fraction);

            Assert.Equal(DropPosition.After, MarkOf(screen, last));
            Assert.Null(MarkOf(screen, lastRow));
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(screen.RowOf(lastRow)));
        }

        [AvaloniaTheory]
        [InlineData(0.6)]
        [InlineData(0.95)]
        public void DragOver_ACompositionOnTheBottomHalfOfAnOpenCompositionRow_MarksNothing(double fraction)
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];

            screen.DragOver(screen.Tree.CompositionNodes()[0], last, fraction);

            Assert.False(IsMarkedBetween(screen, last));
            Assert.DoesNotContain(":droptarget", screen.RowOf(last).Classes);
        }

        [AvaloniaTheory]
        [InlineData(0.1)]
        [InlineData(0.9)]
        public void DragOver_ACompositionOnAMiddleRowOfABlock_MarksNothing(double fraction)
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];

            screen.DragOver(screen.Tree.CompositionNodes()[0], last.Children[0], fraction);

            Assert.False(IsMarkedBetween(screen, last));
        }

        [AvaloniaFact]
        public void DragOver_ACollapsedCompositionRow_UsesBothHalves()
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];
            last.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            screen.Window.UpdateLayout();

            screen.DragOver(screen.Tree.CompositionNodes()[0], last, 0.9);

            Assert.Equal(DropPosition.After, MarkOf(screen, last));
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(screen.RowOf(last)));
        }

        [AvaloniaFact]
        public void DragOver_AfterABlock_DrawsTheBarUnderItsLastRow()
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];
            var lastRow = LastVisibleOf(last);

            screen.DragOver(screen.Tree.CompositionNodes()[0], lastRow, 0.9);

            var line = DropLineOf(screen.RowOf(lastRow));
            var lastRowHeader = HeaderOf(screen.RowOf(lastRow));
            var gap = Convert.ToDouble(screen.View.Resources["TreeRowGap"]);
            var thin = Convert.ToDouble(screen.View.Resources["TreeDropLineThickness"]);
            Assert.True(line.IsVisible);
            // The bar is one pixel in the middle of the space under the last row of the block.
            Assert.Equal(Top(screen, lastRowHeader) + lastRowHeader.Bounds.Height + (gap - thin) / 2, Top(screen, line), 3);
            Assert.Equal(thin, line.Bounds.Height, 3);
        }

        [AvaloniaFact]
        public void DragOver_BeforeABlock_DrawsTheBarAtItsTop()
        {
            var screen = ShowWithABlock();
            var last = screen.Tree.CompositionNodes()[2];

            screen.DragOver(screen.Tree.CompositionNodes()[0], last, 0.2);

            var above = HeaderOf(screen.RowOf(LastVisibleOf(screen.Tree.CompositionNodes()[1])));
            var below = HeaderOf(screen.RowOf(last));
            var line = DropLineOf(screen.RowOf(LastVisibleOf(screen.Tree.CompositionNodes()[1])));
            Assert.True(line.IsVisible);
            // The bar is in the middle of the space between the two rows: the same space above it and under it.
            Assert.Equal(Top(screen, line) - (Top(screen, above) + above.Bounds.Height), Top(screen, below) - (Top(screen, line) + line.Bounds.Height), 3);
            Assert.True(Top(screen, line) > Top(screen, above) + above.Bounds.Height - 0.001);
        }

        [AvaloniaFact]
        public void DragOver_ALayerOnTheBottomHalfOfTheLastRowOfALayer_MarksTheDropAfterIt()
        {
            var screen = Show();
            var first = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var second = screen.Tree.CompositionNodes()[0].Children[0].Children[1];
            var lastRow = LastVisibleOf(second);
            Assert.NotSame(second, lastRow);

            screen.DragOver(first, lastRow, 0.9);

            Assert.Equal(DropPosition.After, MarkOf(screen, second));
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(screen.RowOf(lastRow)));
        }

        [AvaloniaFact]
        public void DragOver_ALayerOnAMiddleRowOfALayer_MarksNothing()
        {
            var screen = Show();
            var first = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var second = screen.Tree.CompositionNodes()[0].Children[0].Children[1];

            screen.DragOver(first, second.Children[0], 0.9);

            Assert.False(IsMarkedBetween(screen, second));
        }

        [AvaloniaTheory]
        [InlineData(0.05)]
        [InlineData(0.5)]
        [InlineData(0.95)]
        public void DragOver_ALayerOnACompositionRow_MarksTheDropInside(double fraction)
        {
            var screen = Show();

            screen.DragOver(screen.Tree.CompositionNodes()[0].Children[0].Children[0], screen.Tree.CompositionNodes()[1], fraction);

            Assert.Contains(":droptarget", screen.RowOf(screen.Tree.CompositionNodes()[1]).Classes);
        }

        private static TextBoxEditable NameBoxOf(TreeViewItem row) =>
            row.GetVisualDescendants().OfType<TextBoxEditable>().First();

        private static bool IsRenaming(TextBoxEditable box) =>
            box.GetVisualDescendants().OfType<TextBox>().Any(input => input.IsVisible);

        // A point in the name box of a row, to the right of the text of the name.
        private static Point AwayFromTheName(Screen screen, TreeViewItem row)
        {
            var box = NameBoxOf(row);
            return box.TranslatePoint(new Point(box.Bounds.Width - 12, box.Bounds.Height / 2), screen.Window)!.Value;
        }

        private static void DoubleClick(Screen screen, Point point)
        {
            // The hit test of the window reads the last rendered scene.
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            screen.Window.MouseDown(point, MouseButton.Left);
            screen.Window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DoubleClick_AwayFromTheName_ExpandsOrCollapsesTheRowWithoutARename(bool expanded)
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            node.IsExpanded = expanded;
            Dispatcher.UIThread.RunJobs();
            var row = screen.RowOf(node);

            DoubleClick(screen, AwayFromTheName(screen, row));

            Assert.Equal(!expanded, row.IsExpanded);
            Assert.Equal(!expanded, node.IsExpanded);
            Assert.False(IsRenaming(NameBoxOf(row)));
        }

        [AvaloniaFact]
        public void DoubleClick_OnTheIconOfARow_ExpandsOrCollapsesTheRow()
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            var row = screen.RowOf(node);
            var icon = row.GetVisualDescendants().OfType<CMiX.UI.Controls.AppIcon>().First();
            var point = icon.TranslatePoint(new Point(icon.Bounds.Width / 2, icon.Bounds.Height / 2), screen.Window)!.Value;

            DoubleClick(screen, point);

            Assert.False(row.IsExpanded);
        }

        [AvaloniaFact]
        public void DoubleClick_AwayFromTheNameOfARowWithoutChildren_DoesNothing()
        {
            var screen = Show();
            // A row without children: a modifier of an entity.
            var layer = screen.A.LayerManager.ManagerData.Items.OfType<Layer>().First();
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var modifiers = ((Entity)layer.ModelEntityManager.ManagerData.Items[0]).ModifierManager;
            modifiers.AddItem(typeof(CMiX.Core.Modulation.Modifiers.TranslateModifier));
            Dispatcher.UIThread.RunJobs();
            var node = screen.Tree.CompositionNodes()[0].Children[0].Children[0].Children[0].Children[0].Children.Single(group => group.Group is ModifiersGroup).Children[0];
            for (var ancestor = node.Parent; ancestor != null; ancestor = ancestor.Parent)
                ancestor.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            var row = screen.RowOf(node);
            var before = node.IsExpanded;

            DoubleClick(screen, AwayFromTheName(screen, row));

            Assert.False(IsRenaming(NameBoxOf(row)));
            Assert.Equal(before, node.IsExpanded);
            Assert.True(screen.RowOf(screen.Tree.CompositionNodes()[0]).IsExpanded);
        }

        [AvaloniaFact]
        public void DoubleClick_OnAToggle_KeepsTheRowAsItIs()
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            var row = screen.RowOf(node);
            var visibility = ToggleWithTheme(row, "ToggleButtonVisibility");
            var before = screen.A.PrefabService.Visibility.Value;
            var point = visibility.TranslatePoint(new Point(visibility.Bounds.Width / 2, visibility.Bounds.Height / 2), screen.Window)!.Value;

            DoubleClick(screen, point);

            Assert.True(row.IsExpanded);
            Assert.Equal(before, screen.A.PrefabService.Visibility.Value);
        }

        [AvaloniaFact]
        public void DoubleClick_InTheInputWhileRenaming_KeepsTheRenameAndTheRow()
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            var row = screen.RowOf(node);
            var box = NameBoxOf(row);
            DoubleClick(screen, box.TranslatePoint(new Point(10, box.Bounds.Height / 2), screen.Window)!.Value);
            Assert.True(IsRenaming(box));

            DoubleClick(screen, box.TranslatePoint(new Point(box.Bounds.Width - 12, box.Bounds.Height / 2), screen.Window)!.Value);

            Assert.True(IsRenaming(box));
            Assert.True(row.IsExpanded);
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void DoubleClick_OnTheName_StartsTheRenameAndKeepsTheRow(bool expanded)
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            node.IsExpanded = expanded;
            Dispatcher.UIThread.RunJobs();
            var row = screen.RowOf(node);
            var box = NameBoxOf(row);
            var point = box.TranslatePoint(new Point(10, box.Bounds.Height / 2), screen.Window)!.Value;

            DoubleClick(screen, point);

            Assert.Equal(expanded, row.IsExpanded);
            Assert.Equal(expanded, node.IsExpanded);
            Assert.Contains(box.GetVisualDescendants().OfType<TextBox>(), input => input.IsVisible);
        }

        [AvaloniaFact]
        public void Show_TheNameBox_FillsTheColoredRowAfterTheIcon()
        {
            var screen = Show();
            var row = screen.RowOf(screen.Tree.CompositionNodes()[0]);
            var content = row.GetVisualDescendants().OfType<Grid>().First(grid => grid.Name == "RowContent");
            var box = NameBoxOf(row);
            var colored = HeaderOf(row);

            var boxRight = box.TranslatePoint(new Point(box.Bounds.Width, 0), screen.Window)!.Value.X;
            var coloredRight = colored.TranslatePoint(new Point(colored.Bounds.Width, 0), screen.Window)!.Value.X;

            // The box reaches the toggles, which end the colored row.
            var toggles = row.GetVisualDescendants().OfType<TogglesRow>().First().Bounds.Width;
            Assert.True(coloredRight - boxRight <= toggles + 4, $"The name box ends {coloredRight - boxRight} px before the end of the row.");
            Assert.True(box.Bounds.Width > content.Bounds.Width / 2, "The name box is less than half of the row.");
        }

        [AvaloniaFact]
        public void Click_TheChevron_ExpandsAndCollapsesTheRow()
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];
            var row = screen.RowOf(node);
            var chevron = row.GetVisualDescendants().OfType<ToggleButton>().First(toggle => toggle.Name == "PART_ExpandCollapseChevron");

            chevron.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            Assert.False(row.IsExpanded);
            Assert.False(node.IsExpanded);

            chevron.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.True(row.IsExpanded);
            Assert.True(node.IsExpanded);
        }

        [AvaloniaFact]
        public void Hover_ARow_LightsOnlyThatRow()
        {
            var screen = Show();
            var composition = screen.Tree.CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];

            screen.Window.MouseMove(screen.PointIn(layer, 0.5));
            Dispatcher.UIThread.RunJobs();

            Assert.True(HeaderOf(screen.RowOf(layer)).IsPointerOver);
            Assert.False(HeaderOf(screen.RowOf(composition)).IsPointerOver);
        }

        [AvaloniaFact]
        public void Show_ACompositionOrLayerRow_HasTheVisibilityAndTheMaskToggle()
        {
            var screen = Show();

            Assert.Equal(2, TogglesOf(screen.RowOf(screen.Tree.CompositionNodes()[0])).Count);
            Assert.Equal(2, TogglesOf(screen.RowOf(screen.Tree.CompositionNodes()[0].Children[0].Children[0])).Count);
        }

        private static ToggleButton ToggleWithTheme(TreeViewItem row, string themeKey) =>
            TogglesOf(row).Single(toggle => ReferenceEquals(toggle.Theme, Application.Current!.FindResource(themeKey)));

        private static double RightEdge(Screen screen, Control control) =>
            control.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X + control.Bounds.Width;

        private static double LeftEdge(Screen screen, Control control) =>
            control.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X;

        [AvaloniaFact]
        public void Show_TheToggles_PutTheMaskFirstAndTheVisibilityRightOfIt()
        {
            var screen = Show();

            foreach (var node in new[] { screen.Tree.CompositionNodes()[0], screen.Tree.CompositionNodes()[0].Children[0].Children[0] })
            {
                var row = screen.RowOf(node);
                var visibility = ToggleWithTheme(row, "ToggleButtonVisibility");
                var mask = ToggleWithTheme(row, "ToggleButtonMask");

                Assert.True(RightEdge(screen, mask) <= visibility.TranslatePoint(new Point(0, 0), screen.Window)!.Value.X,
                    $"The mask toggle of {node.Header} is not left of the visibility toggle.");
            }
        }

        [AvaloniaFact]
        public void Show_TheVisibilityToggleOfEveryRow_HasTheSameLeftEdge()
        {
            var screen = Show();
            screen.A.LayerManager.ManagerData.Items.OfType<Layer>().First().ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            screen.Window.UpdateLayout();
            var composition = screen.Tree.CompositionNodes()[0];
            var layer = composition.Children[0].Children[0];
            var entity = layer.Children[0].Children[0];

            var edges = new[] { composition, layer, entity }
                .Select(node => LeftEdge(screen, ToggleWithTheme(screen.RowOf(node), "ToggleButtonVisibility")))
                .ToList();

            Assert.All(edges, edge => Assert.Equal(edges[0], edge));
        }

        [AvaloniaFact]
        public void Show_AnEntityRowAndAGroupRow_HaveOnlyTheVisibilityOrTheHideButtonOfTheList()
        {
            var screen = Show();
            screen.A.LayerManager.ManagerData.Items.OfType<Layer>().First().ModelEntityManager.AddItem(typeof(Entity));
            Dispatcher.UIThread.RunJobs();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var group = layerNode.Children[0];

            Assert.Single(TogglesOf(screen.RowOf(group.Children[0])));
            // A group has the hide button of its list, which is a visibility toggle too.
            Assert.Single(TogglesOf(screen.RowOf(group)));
        }

        [AvaloniaTheory]
        [InlineData("ToggleButtonVisibility")]
        [InlineData("ToggleButtonMask")]
        public void Click_TheToggles_ChangeTheVisibilityAndTheMaskOfTheComposition(string themeKey)
        {
            var screen = Show();
            var visibility = themeKey == "ToggleButtonVisibility";
            var toggle = ToggleWithTheme(screen.RowOf(screen.Tree.CompositionNodes()[0]), themeKey);
            var before = visibility ? screen.A.PrefabService.Visibility.Value : screen.A.Mask.IsMask.Value;

            toggle.IsChecked = !before;
            Dispatcher.UIThread.RunJobs();

            var after = visibility ? screen.A.PrefabService.Visibility.Value : screen.A.Mask.IsMask.Value;
            Assert.Equal(!before, after);
        }

        [AvaloniaFact]
        public void Show_ARowOfAGroup_ShowsTheNameOfTheGroup()
        {
            var screen = Show();
            var group = screen.Tree.CompositionNodes()[0].Children[0].Children[0].Children[0];

            var texts = screen.RowOf(group).GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text);

            Assert.Contains("Entities", texts);
        }

        [AvaloniaFact]
        public void Select_ARow_SelectsTheNode()
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0].Children[0].Children[1];

            screen.TreeView.SelectedItem = node;

            Assert.Same(node, screen.Tree.SelectedNode);
            Assert.Same(screen.A.LayerManager.ManagerData.Items[1], screen.A.LayerManager.SelectedItem);
        }

        [AvaloniaFact]
        public void DragOver_TheLowerPartOfARow_MarksTheRowUntilTheDragEnds()
        {
            var screen = Show();
            var row = screen.RowOf(screen.Tree.CompositionNodes()[1]);
            var lastRow = LastVisibleOf(screen.Tree.CompositionNodes()[1]);

            screen.DragOver(screen.Tree.CompositionNodes()[0], lastRow, 0.9);
            Assert.Equal(DropPosition.After, MarkOf(screen, screen.Tree.CompositionNodes()[1]));
            Assert.Equal(DropMark.GapBelow, TreeDropMark.GetMark(screen.RowOf(lastRow)));

            TreeDrag.Cancel(screen.Window, screen.PointIn(lastRow, 0.9));
            Assert.Null(screen.View.Marker.Marked);
            Assert.Equal(DropMark.None, TreeDropMark.GetMark(screen.RowOf(lastRow)));
        }

        [AvaloniaFact]
        public void DragOver_TheMiddleOfACompositionRow_MarksItAsInside()
        {
            var screen = Show();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];

            screen.DragOver(layerNode, screen.Tree.CompositionNodes()[1], 0.5);

            Assert.Contains(":droptarget", screen.RowOf(screen.Tree.CompositionNodes()[1]).Classes);
        }

        [AvaloniaTheory]
        [InlineData(0.1)]
        [InlineData(0.9)]
        public void DragOver_TheItemItself_MarksNothing(double fraction)
        {
            var screen = Show();
            var node = screen.Tree.CompositionNodes()[0];

            screen.DragOver(node, node, fraction);

            Assert.False(IsMarkedBetween(screen, node));
            Assert.DoesNotContain(":droptarget", screen.RowOf(node).Classes);
        }

        [AvaloniaFact]
        public void DragOver_ThePlaceOfTheItem_MarksNothing()
        {
            var screen = Show();
            var next = screen.Tree.CompositionNodes()[1];

            // The first composition before the second one: the same place.
            screen.DragOver(screen.Tree.CompositionNodes()[0], next, 0.1);

            Assert.NotEqual(DropPosition.Before, MarkOf(screen, next));
        }

        // The tree draws the cursor of the drag itself. The cursor for a drop that is not possible does not fit the look of the app:
        // the mark tells where a drop works.
        [AvaloniaTheory]
        [InlineData(0.5, true)]
        [InlineData(0.1, false)]
        public void DragOver_ANodeOfTheTree_ShowsOnlyTheMoveCursor(double fraction, bool allowed)
        {
            var screen = Show();
            var source = screen.Tree.CompositionNodes()[0];
            var target = allowed ? screen.Tree.CompositionNodes()[1] : source;

            screen.DragOver(source, target, fraction);

            Assert.Same(screen.View.RowDrag.MoveCursor, screen.TreeView.Cursor);
        }

        [AvaloniaFact]
        public void DragOver_ATargetThatIsNotAllowed_MarksNothing()
        {
            var screen = Show();
            var group = screen.Tree.CompositionNodes()[0].Children[0].Children[0].Children[0];

            screen.DragOver(screen.Tree.CompositionNodes()[0], group, 0.5);

            Assert.DoesNotContain(":droptarget", screen.RowOf(group).Classes);
            Assert.False(IsMarkedBetween(screen, group));
        }

        [AvaloniaFact]
        public void Drop_OnTheBottomHalfOfTheLastRowOfACompositionBlock_ReordersTheCompositions()
        {
            var screen = Show();
            var nodeA = screen.Tree.CompositionNodes()[0];
            var nodeB = screen.Tree.CompositionNodes()[1];

            screen.Drop(nodeA, LastVisibleOf(nodeB), 0.9);

            Assert.Equal(new IControl[] { screen.B, screen.A }, screen.Project.CompositionManager.ManagerData.Items);
            Assert.NotEqual(DropPosition.After, MarkOf(screen, nodeB));
        }

        [AvaloniaFact]
        public void Drop_ALayerOnTheMiddleOfACompositionRow_MovesTheLayerThere()
        {
            var screen = Show();
            var moved = screen.A.LayerManager.ManagerData.Items[0];

            screen.Drop(screen.Tree.CompositionNodes()[0].Children[0].Children[0], screen.Tree.CompositionNodes()[1], 0.5);

            Assert.Single(screen.A.LayerManager.ManagerData.Items);
            Assert.Same(moved, screen.B.LayerManager.ManagerData.Items[1]);
        }

        [AvaloniaFact]
        public void Drop_ALayerWithASharedEntity_ChangesNothing()
        {
            var screen = Show();
            var layer = (Layer)screen.A.LayerManager.ManagerData.Items[0];
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (IPrefab)layer.ModelEntityManager.ManagerData.Items[0];
            ((Layer)screen.A.LayerManager.ManagerData.Items[1]).ModelEntityManager.AddExistingItem(entity);

            screen.Drop(screen.Tree.CompositionNodes()[0].Children[0].Children[0], screen.Tree.CompositionNodes()[1], 0.5);

            Assert.Equal(2, screen.A.LayerManager.ManagerData.Items.Count);
            Assert.Single(screen.B.LayerManager.ManagerData.Items);
        }

        [AvaloniaFact]
        public void CanDrag_AGroupOrAnItem_IsTrueForTheItemOnly()
        {
            var screen = Show();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            ((Layer)screen.A.LayerManager.ManagerData.Items[0]).ModelEntityManager.AddItem(typeof(Entity));

            foreach (var group in layerNode.Children)
                Assert.False(screen.Tree.CanDrag(group));
            Assert.True(screen.Tree.CanDrag(screen.Tree.CompositionNodes()[0]));
            Assert.True(screen.Tree.CanDrag(layerNode));
            Assert.True(screen.Tree.CanDrag(layerNode.Children[0].Children[0]));
        }

        // The press on a row remembers the node for a drag. A group is not remembered, so a move of the pointer starts no drag.
        [AvaloniaFact]
        public void Press_OnAGroupOrALayer_PreparesADragForTheLayerOnly()
        {
            var screen = Show();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var pointerInput = typeof(TreeViewProject).GetField("pointerInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(screen.View)!;
            var field = typeof(TreePointerInput).GetField("pressedDraggableNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();

            screen.Window.MouseDown(screen.PointIn(layerNode.Children[0], 0.5), MouseButton.Left);
            Assert.Null(field.GetValue(pointerInput));
            screen.Window.MouseUp(screen.PointIn(layerNode.Children[0], 0.5), MouseButton.Left);

            screen.Window.MouseDown(screen.PointIn(layerNode, 0.5), MouseButton.Left);
            Assert.Same(layerNode, field.GetValue(pointerInput));
            screen.Window.MouseUp(screen.PointIn(layerNode, 0.5), MouseButton.Left);
        }

        // The nodes that the view reports as chosen.
        private static List<TreeNode> RecordChosen(Screen screen)
        {
            var chosen = new List<TreeNode>();
            screen.View.NodeChosen += (_, e) => chosen.Add(e.Node);
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            return chosen;
        }

        [AvaloniaFact]
        public void PointerPressed_ThatStartsADrag_RaisesNoNodeChosen()
        {
            var screen = Show();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var chosen = RecordChosen(screen);
            var start = screen.PointIn(layerNode, 0.5);
            var end = start + new Vector(0, 20);

            screen.Window.MouseDown(start, MouseButton.Left);
            Assert.Empty(chosen);
            screen.Window.MouseMove(end, RawInputModifiers.LeftMouseButton);
            screen.Window.MouseUp(end, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            Assert.Empty(chosen);
        }

        [AvaloniaFact]
        public void PointerReleased_WithoutADrag_RaisesNodeChosen()
        {
            var screen = Show();
            var layerNode = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var chosen = RecordChosen(screen);
            var point = screen.PointIn(layerNode, 0.5);

            screen.Window.MouseDown(point, MouseButton.Left);
            Assert.Empty(chosen);
            screen.Window.MouseUp(point, MouseButton.Left);

            Assert.Equal(new[] { layerNode }, chosen);
        }

        // A click on the row of the node, with or without Ctrl.
        private static void Click(Screen screen, TreeNode node, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            screen.Window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
            var point = screen.PointIn(node, 0.5);
            screen.Window.MouseMove(point, modifiers);
            screen.Window.MouseDown(point, MouseButton.Left, modifiers);
            screen.Window.MouseUp(point, MouseButton.Left, modifiers);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void CtrlClick_AnotherRow_KeepsTheSelection()
        {
            var screen = Show();
            var first = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var second = screen.Tree.CompositionNodes()[0].Children[0].Children[1];
            Click(screen, first);
            Assert.Same(first, screen.Tree.SelectedNode);

            Click(screen, second, RawInputModifiers.Control);

            Assert.Same(first, screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void CtrlClick_TheSelectedRow_KeepsItSelected()
        {
            var screen = Show();
            var first = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            Click(screen, first);

            Click(screen, first, RawInputModifiers.Control);

            Assert.Same(first, screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Click_AnotherRowAfterACtrlClick_SelectsThatRow()
        {
            var screen = Show();
            var first = screen.Tree.CompositionNodes()[0].Children[0].Children[0];
            var second = screen.Tree.CompositionNodes()[0].Children[0].Children[1];
            Click(screen, first);
            Click(screen, second, RawInputModifiers.Control);

            Click(screen, second);

            Assert.Same(second, screen.Tree.SelectedNode);
        }

        [AvaloniaFact]
        public void Drop_OnATargetThatIsNotAllowed_ChangesNothing()
        {
            var screen = Show();
            var group = screen.Tree.CompositionNodes()[0].Children[0].Children[0].Children[0];

            screen.Drop(screen.Tree.CompositionNodes()[1], group, 0.5);

            Assert.Equal(new IControl[] { screen.A, screen.B }, screen.Project.CompositionManager.ManagerData.Items);
        }

        // The number of handlers of the change event of the tree that belong to the view.
        private static int HandlersOfView(ProjectTree tree, TreeViewProject view)
        {
            var field = typeof(CommunityToolkit.Mvvm.ComponentModel.ObservableObject)
                .GetField("PropertyChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            var handler = (Delegate?)field.GetValue(tree);
            return handler?.GetInvocationList().Count(item => ReferenceEquals(item.Target, view)) ?? 0;
        }

        [AvaloniaFact]
        public void Detach_AnAttachedView_RemovesTheViewModelHandlers()
        {
            var screen = Show();
            Assert.Equal(1, HandlersOfView(screen.Tree, screen.View));

            screen.Window.Content = null;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0, HandlersOfView(screen.Tree, screen.View));
        }

        [AvaloniaFact]
        public void Attach_Again_ListensToTheViewModelAgain()
        {
            var screen = Show();
            screen.Window.Content = null;
            Dispatcher.UIThread.RunJobs();

            screen.Window.Content = screen.View;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, HandlersOfView(screen.Tree, screen.View));
        }
    }
}
