// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using Xunit;
using static CMiX.Studio.Avalonia.Tests.Support.ListTreeScenes;
using static CMiX.Studio.Avalonia.Tests.Support.TreeFixtures;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ListTreeCandidatesTests
    {
        private readonly TreeDropService service = new();

        // Layer 1 of A has two entities with modifiers, a camera and a filter. Layer 2 of A has only a filter.
        // B has an empty layer and a layer with one entity. Every empty list is closed.
        private static Scene CreateScene() => Opened(TreeFixtures.CreateScene([2, 2], Fill));

        // The second entity of layer 1 of A shows under layer 2 of A too.
        private static Scene CreateSharedScene() => Opened(TreeFixtures.CreateScene([2, 2], ShareAnEntity));

        private IReadOnlyList<ListTreeCandidate> CandidatesOf(Scene scene, TreeNode source)
        {
            var snapshot = ListTreeSnapshot.Of(scene.Tree.Roots);
            var candidates = new ListTreeCandidates(new ListTreeSlots(service), service, ListTreeSizes.Default);
            return candidates.Of(snapshot, snapshot.VisibleRows().Single(row => ReferenceEquals(row.Node, source)));
        }

        // A row with no node at the end of the list, as a copy that the live tree does not have yet.
        private static ListTreeSnapshot WithACopyRow(ListTreeSnapshot live, ListTreeRowKey list) =>
            new(live.Roots.Select(row => WithACopyRowIn(row, list)).ToArray());

        private static ListTreeRow WithACopyRowIn(ListTreeRow row, ListTreeRowKey list)
        {
            var children = row.Children.Select(child => WithACopyRowIn(child, list)).ToList();
            if (row.Key.Equals(list))
                children.Add(new ListTreeRow(new ListTreeRowKey(new object(), row.Key), null, row.Depth + 1, false, false, false, []));

            return row with { Children = children };
        }

        private static ListTreeRow CopyRowOf(ListTreeSnapshot snapshot) => snapshot.VisibleRows().Single(row => row.Node == null);

        private ListTreeCandidates Candidates() => new(new ListTreeSlots(service), service, ListTreeSizes.Default);

        private IReadOnlyList<ListTreeSlot> SlotsOf(Scene scene, TreeNode source) =>
            new ListTreeSlots(service).Of(ListTreeSnapshot.Of(scene.Tree.Roots), source);

        private static ListTreePlace LivePlaceOf(Scene scene, TreeNode node) =>
            ListTreeLayout.Of(ListTreeSnapshot.Of(scene.Tree.Roots), ListTreeSizes.Default).PlaceOf(ListTreeKeys.Of(node))!;

        private static void AssertYAndDepth(ListTreePlace expected, ListTreePlace actual)
        {
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.Depth, actual.Depth);
        }

        private static bool IsInComposition(ListTreeSlot slot, Composition composition) =>
            ReferenceEquals(slot.ListPlace.List, composition.LayerManager)
            || composition.LayerManager.ManagerData.Items.OfType<Layer>().Any(layer => ReferenceEquals(slot.ListPlace.List, layer.ModelEntityManager));

        [Fact]
        public void Of_AnEntity_GivesOneCandidatePerLegalSlotInTreeOrder()
        {
            var scene = CreateScene();
            var source = Entity(scene, 0);

            var candidates = CandidatesOf(scene, source);

            Assert.Equal(SlotsOf(scene, source), candidates.Select(candidate => candidate.Slot));
        }

        [Fact]
        public void Of_ASlotBeforeTheFirstEntity_GivesThePlaceOfTheFirstEntity()
        {
            var scene = CreateScene();
            var first = Entity(scene, 0);

            var candidates = CandidatesOf(scene, Entity(scene, 1));

            var before = Assert.Single(candidates, candidate => ReferenceEquals(candidate.Slot.Target, first) && candidate.Slot.Position == DropPosition.Before);
            AssertYAndDepth(LivePlaceOf(scene, first), before.Place);
        }

        [Fact]
        public void Of_ASlotInAnotherComposition_GivesThePlaceOfTheMovedRowInThatLayout()
        {
            var scene = CreateScene();
            var end = new ListPlace(LayerOfB(scene, 1).ModelEntityManager, 1);
            var entityOfB = Scene.Group<EntitiesGroup>(scene.LayerNodeOfB(1)).Children[0];
            var rowsThatLeave = 4;

            var candidates = CandidatesOf(scene, Entity(scene, 0));

            var place = Assert.Single(candidates, candidate => candidate.Slot.ListPlace == end).Place;
            var below = LivePlaceOf(scene, entityOfB);
            Assert.Equal(below.Y + (1 - rowsThatLeave) * ListTreeSizes.Default.Pitch, place.Y);
            Assert.Equal(below.Depth, place.Depth);
        }

        [Fact]
        public void Of_TheCurrentSlot_GivesThePlaceOfTheSource()
        {
            var scene = CreateScene();
            var source = Entity(scene, 1);

            var candidates = CandidatesOf(scene, source);

            var current = Assert.Single(candidates, candidate => ReferenceEquals(candidate.Slot.Target, source));
            AssertYAndDepth(LivePlaceOf(scene, source), current.Place);
        }

        [Fact]
        public void Of_ALayerThatSharesItems_GivesNoSlotInAnotherComposition()
        {
            var scene = CreateSharedScene();
            var source = scene.LayerNodeOfA(0);

            var candidates = CandidatesOf(scene, source);

            Assert.Contains(SlotsOf(scene, source), slot => IsInComposition(slot, scene.B));
            Assert.DoesNotContain(candidates, candidate => IsInComposition(candidate.Slot, scene.B));
            Assert.Contains(candidates, candidate => IsInComposition(candidate.Slot, scene.A));
        }

        [Fact]
        public void Of_ASnapshotWithARowWithNoNode_SkipsThatRow()
        {
            var scene = CreateScene();
            var snapshot = WithACopyRow(ListTreeSnapshot.Of(scene.Tree.Roots), ListTreeKeys.Of(scene.Group<EntitiesGroup>(0)));
            var copy = CopyRowOf(snapshot);

            var candidates = Candidates().Of(snapshot, snapshot.VisibleRows().Single(row => ReferenceEquals(row.Node, Entity(scene, 0))));

            Assert.NotEmpty(candidates);
            Assert.DoesNotContain(candidates, candidate => candidate.Slot.ShownTarget.Equals(copy.Key));
        }

        [Fact]
        public void Of_ASourceWithNoNode_GivesNoCandidate()
        {
            var scene = CreateScene();
            var snapshot = WithACopyRow(ListTreeSnapshot.Of(scene.Tree.Roots), ListTreeKeys.Of(scene.Group<EntitiesGroup>(0)));

            var candidates = Candidates().Of(snapshot, CopyRowOf(snapshot));

            Assert.Empty(candidates);
        }

        [Fact]
        public void Of_AnItemOfALayerThatSharesIt_GivesNoSlotInAnotherComposition()
        {
            var scene = CreateSharedScene();
            var source = Entity(scene, 1);

            var candidates = CandidatesOf(scene, source);

            Assert.Contains(SlotsOf(scene, source), slot => IsInComposition(slot, scene.B));
            Assert.DoesNotContain(candidates, candidate => IsInComposition(candidate.Slot, scene.B));
            Assert.Contains(candidates, candidate => IsInComposition(candidate.Slot, scene.A));
        }
    }
}
