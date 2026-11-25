using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    // The search of the project tree: a row matches on its name or on the kind of its item. While a search runs, the parents of a match hold a match below.
    public class ProjectTreeSearchTests
    {
        private static void Rename(object item, string name) => ((IPrefab)item).PrefabService.Name.Value = name;

        private static TreeNode NodeOf(Scene scene, object item) => scene.All().Single(node => ReferenceEquals(node.Item, item));

        // A with two layers. The first layer holds an Echo filter named Trail.
        private static (Scene Scene, Echo Echo) WithARenamedEcho()
        {
            Echo? echo = null;
            var scene = CreateScene([2], s =>
            {
                s.Layer.TextureModifierManager.AddItem(typeof(Echo));
                echo = (Echo)s.Layer.TextureModifierManager.ManagerData.Items[0];
                Rename(echo, "Trail");
            });
            return (scene, echo!);
        }

        [Fact]
        public void Search_AnEmptyText_MatchesNothing()
        {
            var (scene, _) = WithARenamedEcho();

            scene.Tree.SearchText = "Echo";
            scene.Tree.SearchText = "";

            Assert.Empty(scene.Tree.SearchMatches);
            Assert.Equal(0, scene.Tree.SearchMatchCount);
            Assert.All(scene.All(), node => Assert.False(node.IsSearchMatch));
        }

        [Fact]
        public void Search_AnEmptyText_ClearsTheParents()
        {
            var (scene, _) = WithARenamedEcho();

            scene.Tree.SearchText = "Echo";
            Assert.Contains(scene.All(), node => node.HasSearchMatchBelow);
            scene.Tree.SearchText = "";

            Assert.All(scene.All(), node => Assert.False(node.HasSearchMatchBelow));
        }

        [Fact]
        public void Clear_ASearchWithNoMatch_MatchesNoRow()
        {
            var (scene, _) = WithARenamedEcho();

            scene.Tree.SearchText = "NoSuchName";
            Assert.Empty(scene.Tree.SearchMatches);
            Assert.All(scene.All(), node => Assert.False(node.IsSearchMatch || node.HasSearchMatchBelow));

            scene.Tree.SearchText = "";

            Assert.All(scene.All(), node => Assert.False(node.IsSearchMatch || node.HasSearchMatchBelow));
        }

        [Fact]
        public void Search_AName_MatchesTheRowWithThatName()
        {
            var scene = CreateScene([2]);
            Rename(scene.LayerOfA(1), "Kick");

            scene.Tree.SearchText = "Kick";

            Assert.Equal(new[] { scene.LayerNodeOfA(1) }, scene.Tree.SearchMatches);
            Assert.True(scene.LayerNodeOfA(1).IsSearchMatch);
        }

        [Fact]
        public void Search_TheKindOfARenamedItem_FindsTheItem()
        {
            var (scene, echo) = WithARenamedEcho();

            scene.Tree.SearchText = "Echo";

            Assert.Equal(new[] { NodeOf(scene, echo) }, scene.Tree.SearchMatches);
        }

        [Fact]
        public void Search_ATextWithOtherCaseAndSpaces_IsTrimmedAndIgnoresTheCase()
        {
            var (scene, echo) = WithARenamedEcho();

            scene.Tree.SearchText = "  eCHo ";

            Assert.Equal(new[] { NodeOf(scene, echo) }, scene.Tree.SearchMatches);
        }

        [Fact]
        public void Search_AGroupRow_MatchesOnItsHeaderOnly()
        {
            var scene = CreateScene([1]);
            var filters = scene.Group<FiltersGroup>(0);

            scene.Tree.SearchText = filters.Header;

            Assert.True(filters.IsSearchMatch);
        }

        [Fact]
        public void Search_AMatch_MatchesOnlyThatRow()
        {
            var (scene, echo) = WithARenamedEcho();
            var match = NodeOf(scene, echo);

            scene.Tree.SearchText = "Echo";

            Assert.All(scene.All().Where(node => !ReferenceEquals(node, match)), node => Assert.False(node.IsSearchMatch));
            Assert.True(match.IsSearchMatch);
        }

        [Fact]
        public void Search_AName_MarksTheParentsOfTheMatch()
        {
            var (scene, echo) = WithARenamedEcho();
            var match = NodeOf(scene, echo);
            var parents = TreeWalk.ParentsOf(match).ToList();

            scene.Tree.SearchText = "Echo";

            Assert.Contains(parents, parent => scene.Tree.Roots.Contains(parent));
            Assert.All(parents, parent => Assert.True(parent.HasSearchMatchBelow));
            Assert.All(scene.All().Where(node => !parents.Contains(node)), node => Assert.False(node.HasSearchMatchBelow));
        }

        [Fact]
        public void SearchMatches_OfTwoRows_AreCountedInTheOrderOfTheTree()
        {
            var scene = CreateScene([2], s =>
            {
                s.LayerOfA(1).TextureModifierManager.AddItem(typeof(Echo));
                s.LayerOfA(0).TextureModifierManager.AddItem(typeof(Echo));
            });
            var first = NodeOf(scene, scene.LayerOfA(0).TextureModifierManager.ManagerData.Items[0]);
            var second = NodeOf(scene, scene.LayerOfA(1).TextureModifierManager.ManagerData.Items[0]);

            scene.Tree.SearchText = "echo";

            Assert.Equal(2, scene.Tree.SearchMatchCount);
            Assert.Equal(new[] { first, second }, scene.Tree.SearchMatches);
        }

        [Fact]
        public void Rename_DuringASearch_MakesARowMatchAndStopMatching()
        {
            var scene = CreateScene([2]);
            scene.Tree.SearchText = "Kick";
            Assert.Empty(scene.Tree.SearchMatches);

            Rename(scene.LayerOfA(0), "Kick drum");
            Assert.Equal(new[] { scene.LayerNodeOfA(0) }, scene.Tree.SearchMatches);
            Assert.True(scene.LayerNodeOfA(0).IsSearchMatch);

            Rename(scene.LayerOfA(0), "Snare");
            Assert.Empty(scene.Tree.SearchMatches);
            Assert.False(scene.LayerNodeOfA(0).IsSearchMatch);
        }

        [Fact]
        public void AddItem_DuringASearch_CountsANewMatch()
        {
            var scene = CreateScene([1]);
            scene.Tree.SearchText = "Echo";
            Assert.Equal(0, scene.Tree.SearchMatchCount);

            scene.Layer.TextureModifierManager.AddItem(typeof(Echo));

            var added = NodeOf(scene, scene.Layer.TextureModifierManager.ManagerData.Items[0]);
            Assert.Equal(1, scene.Tree.SearchMatchCount);
            Assert.True(added.IsSearchMatch);
        }

        [Fact]
        public void RemoveItem_DuringASearch_TakesTheMatchAway()
        {
            var (scene, echo) = WithARenamedEcho();
            scene.Tree.SearchText = "Echo";
            Assert.Equal(1, scene.Tree.SearchMatchCount);

            scene.Layer.TextureModifierManager.RemoveItem(echo);

            Assert.Equal(0, scene.Tree.SearchMatchCount);
            Assert.Empty(scene.Tree.SearchMatches);
        }

        [Fact]
        public void LeaveSearch_WithAText_EmptiesTheTextAndKeepsTheFocus()
        {
            var scene = CreateScene([1]);
            var focusRequests = 0;
            scene.Tree.TreeFocusRequested += (_, _) => focusRequests++;
            scene.Tree.SearchText = "Echo";

            scene.Tree.LeaveSearch();

            Assert.Equal(string.Empty, scene.Tree.SearchText);
            Assert.Equal(0, focusRequests);
        }

        [Fact]
        public void LeaveSearch_WithNoText_RequestsTheTreeFocus()
        {
            var scene = CreateScene([1]);
            var focusRequests = 0;
            scene.Tree.TreeFocusRequested += (_, _) => focusRequests++;

            scene.Tree.LeaveSearch();

            Assert.Equal(1, focusRequests);
        }

        [Fact]
        public void LeaveSearch_DuringADrag_CancelsTheDrag()
        {
            var scene = CreateScene([2]);
            var cancels = 0;
            scene.Tree.AttachView(new TreeViewHooks { CancelDrag = () => cancels++ });
            scene.Tree.SearchText = "Echo";
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));

            scene.Tree.LeaveSearch();

            Assert.Equal(1, cancels);
            Assert.Equal("Echo", scene.Tree.SearchText);
        }

        [Fact]
        public void MoveCursorToNextMatch_DuringADrag_CannotRun()
        {
            var scene = CreateScene([2]);
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));

            Assert.False(scene.Tree.MoveCursorToNextMatchCommand.CanExecute(null));
            Assert.False(scene.Tree.MoveCursorToPreviousMatchCommand.CanExecute(null));
        }

        [Fact]
        public void MoveCursorToPreviousMatch_TwoMatches_MovesToTheMatchBefore()
        {
            var scene = CreateScene([2], s =>
            {
                s.LayerOfA(1).TextureModifierManager.AddItem(typeof(Echo));
                s.LayerOfA(0).TextureModifierManager.AddItem(typeof(Echo));
            });
            var first = NodeOf(scene, scene.LayerOfA(0).TextureModifierManager.ManagerData.Items[0]);
            var second = NodeOf(scene, scene.LayerOfA(1).TextureModifierManager.ManagerData.Items[0]);
            scene.Tree.SearchText = "echo";
            scene.Tree.CursorNode = second;

            scene.Tree.MoveCursorToPreviousMatch();

            Assert.Same(first, scene.Tree.CursorNode);
        }

        [Fact]
        public void FocusTreeCommand_DuringADrag_CannotRun()
        {
            var scene = CreateScene([2]);
            scene.Tree.BeginDrag(scene.LayerNodeOfA(0));

            Assert.False(scene.Tree.FocusTreeCommand.CanExecute(null));
        }
    }
}
