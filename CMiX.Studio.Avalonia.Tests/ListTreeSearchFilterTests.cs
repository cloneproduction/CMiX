// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Headless.XUnit;
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

        // The search of today still opens the parents of a match. The rows that hold no match keep IsExpanded false.
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
            Assert.All(scene.All().Where(node => node.Children.Count > 0 && !node.HasSearchMatchBelow), node => Assert.False(node.IsExpanded));
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
