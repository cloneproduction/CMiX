// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // During a search the rows show only the matches and their parents, and the parents show open.
    public class ListTreeSearchFilterTests
    {
        // A with two layers and B with one. The first layer of A holds an Echo filter named Trail.
        private static Scene WithARenamedEcho() =>
            CreateScene([2, 1], s =>
            {
                s.Layer.TextureModifierManager.AddItem(typeof(CMiX.Core.Texturing.Filters.Echo));
                ((IPrefab)s.Layer.TextureModifierManager.ManagerData.Items[0]).PrefabService.Name.Value = "Trail";
            });

        private static TreeNode[] ShownNodes(ListTreeProject view) => view.Rows.LiveRows.Select(row => row.Node!).ToArray();

        private static TextBlock NoMatchLine(ListTreeProject view) =>
            view.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "noMatchLine");

        private static TreeNode[] MatchesAndParents(Scene scene) => scene.All().Where(node => node.IsSearchMatch || node.HasSearchMatchBelow).ToArray();

        [AvaloniaFact]
        public void Filter_ASearch_ShowsOnlyTheMatchesAndTheirParents()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);

            scene.Tree.SearchText = "Trail";
            screen.Settle();

            var shown = ShownNodes(screen.View);
            Assert.Single(shown, node => node.IsSearchMatch);
            Assert.Equal(MatchesAndParents(scene), shown);
            Assert.DoesNotContain(scene.NodeB, shown);
            screen.Window.Close();
        }

        [AvaloniaFact]
        public void Search_NoMatch_ShowsTheNoMatchLine()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);

            scene.Tree.SearchText = "NothingHoldsThis";
            screen.Settle();

            Assert.True(NoMatchLine(screen.View).IsEffectivelyVisible);
            screen.Window.Close();
        }

        [AvaloniaFact]
        public void Search_AMatch_HidesTheNoMatchLine()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            Assert.False(NoMatchLine(screen.View).IsEffectivelyVisible);

            scene.Tree.SearchText = "Trail";
            screen.Settle();

            Assert.False(NoMatchLine(screen.View).IsEffectivelyVisible);
            screen.Window.Close();
        }

        [AvaloniaFact]
        public void Filter_ASearch_ShowsTheParentsOpenAndKeepsIsExpanded()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            scene.Tree.CollapseAll();
            screen.Settle();

            scene.Tree.SearchText = "Trail";
            screen.Settle();

            var parents = screen.View.Rows.LiveRows.Where(row => row.Node!.HasSearchMatchBelow).ToArray();
            Assert.NotEmpty(parents);
            Assert.All(parents, row => Assert.True(row.IsExpanded));
            Assert.All(scene.All().Where(node => node.Children.Count > 0), node => Assert.False(node.IsExpanded));
            screen.Window.Close();
        }

        [AvaloniaFact]
        public void Filter_TheEndOfTheSearch_ShowsTheOwnOpenState()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            scene.Tree.SearchText = "Trail";
            screen.Settle();

            scene.Tree.SearchText = "";
            screen.Settle();

            var own = ListTreeSnapshot.Of(scene.Tree.Roots).VisibleRows();
            Assert.Equal(own.Select(row => row.Node), ShownNodes(screen.View));
            Assert.All(screen.View.Rows.LiveRows, row => Assert.Equal(row.Node!.IsExpanded, row.IsExpanded));
            Assert.Contains(scene.NodeB, ShownNodes(screen.View));
            screen.Window.Close();
        }

        // The filter takes away the focused row, so the focus goes to the cursor row, the first match.
        [AvaloniaFact]
        public void Search_AFocusedRowThatHides_GivesTheFocusToTheCursorRow()
        {
            var scene = WithARenamedEcho();
            var screen = ListTreeTestScreen.Show(scene.Tree, 400, 900);
            screen.Settle();
            screen.View.Rows.RowOf(scene.NodeB)!.Focus();
            screen.Settle();

            scene.Tree.SearchText = "Trail";
            screen.Settle();

            var match = Assert.Single(scene.Tree.SearchMatches);
            Assert.Same(match, scene.Tree.CursorNode);
            Assert.Same(screen.View.Rows.RowOf(match), screen.Window.FocusManager!.GetFocusedElement());
            screen.Window.Close();
        }

        [Fact]
        public void Filter_NoSearch_ShowsEveryRow()
        {
            var scene = WithARenamedEcho();

            Assert.All(scene.All(), node => Assert.True(ListTreeSearchFilter.None.Shows(node)));
            Assert.All(scene.All(), node => Assert.Equal(node.IsExpanded, ListTreeSearchFilter.None.ShowsOpen(node)));
            Assert.Equal(ListTreeSnapshot.Of(scene.Tree.Roots).AllRows().Select(row => row.Key),
                ListTreeSnapshot.Of(scene.Tree.Roots, ListTreeSearchFilter.None).AllRows().Select(row => row.Key));
        }

        [Fact]
        public void Filter_ASearchWithNoMatch_ShowsNoRow()
        {
            var scene = WithARenamedEcho();

            scene.Tree.SearchText = "NoSuchName";

            Assert.Empty(ListTreeSnapshot.Of(scene.Tree.Roots, ListTreeSearchFilter.Matches).Roots);
        }
    }
}
