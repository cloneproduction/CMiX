// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Texturing.Filters;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreePickTests
    {
        private readonly ListTreePick pick = new(ListTreeDragNumbers.Default);

        // Layer 1 of A has two entities and a filter. Layer 2 of A has only a filter. Every empty list is closed.
        private static Scene CreateScene()
        {
            var scene = TreeFixtures.CreateScene([2], Fill);
            foreach (var node in scene.All())
                node.IsExpanded = node.Group == null || node.Children.Count > 0;

            return scene;
        }

        private static void Fill(Scene scene)
        {
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(0).ModelEntityManager.AddItem(typeof(Entity));
            scene.LayerOfA(0).TextureModifierManager.AddItem(typeof(Blur));
            scene.LayerOfA(1).TextureModifierManager.AddItem(typeof(Blur));
        }

        private static TreeNode FilterOfTheSecondLayer(Scene scene) => scene.Group<FiltersGroup>(1).Children[0];

        private static IReadOnlyList<ListTreeCandidate> CandidatesOf(Scene scene, TreeNode source)
        {
            var service = new TreeDropService();
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var candidates = new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default);
            return candidates.Of(snapshot, snapshot.VisibleRows().Single(row => ReferenceEquals(row.Node, source)));
        }

        // A candidate whose moved row is at the spot. The pick reads only the spot.
        private static ListTreeCandidate CandidateAt(double y, int depth)
        {
            var scene = CreateScene();
            return CandidatesOf(scene, FilterOfTheSecondLayer(scene))[0] with { Spot = new ListTreeSpot(y, depth) };
        }

        // The candidates of one scene, each of its own slot. The pick reads only the spot.
        private static IReadOnlyList<ListTreeCandidate> SlotsOfOneScene()
        {
            var scene = CreateScene();
            return CandidatesOf(scene, FilterOfTheSecondLayer(scene));
        }

        private static ListTreeCandidate At(ListTreeCandidate candidate, double y) => candidate with { Spot = new ListTreeSpot(y, 0) };

        private static ListTreePress PressAt(double x, double y) => new(new ListTreePoint(x, y), 0, 0);

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(3, 0, false)]
        [InlineData(4, 0, false)]
        [InlineData(5, 0, true)]
        [InlineData(3, 3, true)]
        [InlineData(0, -5, true)]
        public void PassedThreshold_Moves_TrueOnlyFartherThanTheThreshold(double dx, double dy, bool expected)
        {
            var press = PressAt(10, 10);

            var passed = pick.PassedThreshold(press, new ListTreePoint(10 + dx, 10 + dy));

            Assert.Equal(expected, passed);
        }

        [Fact]
        public void Nearest_ADraggedRowNearASpot_PicksThatCandidate()
        {
            var scene = CreateScene();
            var candidates = CandidatesOf(scene, scene.Group<EntitiesGroup>(0).Children[1]);
            var first = candidates[0];
            var press = new ListTreePress(new ListTreePoint(100, 400), 10, first.Spot.Depth);

            var nearest = pick.Nearest(candidates, press, new ListTreePoint(102, first.Spot.Y + 10 + 3));

            Assert.Same(first, nearest);
        }

        [Fact]
        public void Nearest_TwoPlacesAtTheSameY_PicksThePlaceAtTheDepthOfThePointerX()
        {
            var scene = CreateScene();
            var source = FilterOfTheSecondLayer(scene);
            var candidates = CandidatesOf(scene, source);
            var current = Assert.Single(candidates, candidate => ReferenceEquals(candidate.Slot.Target, source));
            var parentEnd = Assert.Single(candidates, candidate => candidate.Spot.Y == current.Spot.Y && candidate.Spot.Depth < current.Spot.Depth);
            var press = new ListTreePress(new ListTreePoint(100, current.Spot.Y), 0, current.Spot.Depth);
            var levelsUp = current.Spot.Depth - parentEnd.Spot.Depth;

            var atTheStart = pick.Nearest(candidates, press, new ListTreePoint(100, current.Spot.Y));
            var toTheLeft = pick.Nearest(candidates, press, new ListTreePoint(100 - levelsUp * 24, current.Spot.Y));

            Assert.Same(current, atTheStart);
            Assert.Same(parentEnd, toTheLeft);
        }

        [Fact]
        public void Nearest_APointer24PxToTheRight_PicksTheDeeperOfTwoPlacesAtTheSameY()
        {
            var cardEnd = CandidateAt(70, 3);
            var parentEnd = CandidateAt(70, 2);
            var candidates = new[] { parentEnd, cardEnd };
            var press = new ListTreePress(new ListTreePoint(100, 70), 0, 2);

            var atThePress = pick.Nearest(candidates, press, new ListTreePoint(100, 70));
            var toTheRight = pick.Nearest(candidates, press, new ListTreePoint(124, 70));

            Assert.Same(parentEnd, atThePress);
            Assert.Same(cardEnd, toTheRight);
        }

        [Fact]
        public void Nearest_NoCandidate_ThrowsAnArgumentException()
        {
            var candidates = Array.Empty<ListTreeCandidate>();

            var nearest = () => pick.Nearest(candidates, PressAt(0, 0), new ListTreePoint(0, 10));

            Assert.Throws<ArgumentException>(nearest);
        }

        [Fact]
        public void NearestOrCurrent_ANearestLessThan6PxCloser_KeepsTheCurrent()
        {
            var slots = SlotsOfOneScene();
            var current = At(slots[0], 20);
            var other = At(slots[1], 14.5);

            var picked = pick.NearestOrCurrent(new[] { current, other }, current, PressAt(0, 0), new ListTreePoint(0, 10));

            Assert.Same(current, picked);
        }

        [Fact]
        public void NearestOrCurrent_ANearest6PxCloser_PicksTheNearest()
        {
            var slots = SlotsOfOneScene();
            var current = At(slots[0], 20);
            var other = At(slots[1], 14);

            var picked = pick.NearestOrCurrent(new[] { current, other }, current, PressAt(0, 0), new ListTreePoint(0, 10));

            Assert.Same(other, picked);
        }

        [Fact]
        public void NearestOrCurrent_ACurrentThatIsNotInTheList_PicksTheNearest()
        {
            var slots = SlotsOfOneScene();
            var current = At(slots[0], 10);
            var far = At(slots[1], 40);
            var near = At(slots[2], 12);

            var picked = pick.NearestOrCurrent(new[] { far, near }, current, PressAt(0, 0), new ListTreePoint(0, 10));

            Assert.Same(near, picked);
        }

        [Fact]
        public void NearestOrCurrent_ARebuiltCandidateListLessThan6PxCloser_KeepsTheSlotOfTheCurrent()
        {
            var scene = CreateScene();
            var source = FilterOfTheSecondLayer(scene);
            var current = At(CandidatesOf(scene, source)[0], 20);
            var rebuilt = CandidatesOf(scene, source);
            var kept = At(rebuilt[0], 20);
            var other = At(rebuilt[1], 14.5);

            var picked = pick.NearestOrCurrent(new[] { kept, other }, current, PressAt(0, 0), new ListTreePoint(0, 10));

            Assert.Same(kept, picked);
        }
    }
}
