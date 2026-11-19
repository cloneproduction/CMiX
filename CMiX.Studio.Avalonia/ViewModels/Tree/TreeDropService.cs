// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public enum DropPosition
    {
        Before,
        After,
        Inside
    }

    public sealed record DropPlace(TreeNode Target, DropPosition Position);

    public sealed record DropOutcome(bool Moved, IReadOnlyList<IPrefab> Blocking)
    {
        public static DropOutcome None { get; } = new(false, Array.Empty<IPrefab>());

        public static DropOutcome From(MoveResult result) =>
            new(result.Moved, result.Blocking);
    }

    // An item moves in its own list, or to the list of the same kind of another owner.
    // A layer moves to another composition only if it shares no item with a layer that stays.
    public sealed class TreeDropService
    {
        // The fraction is the height of the pointer on the row, from 0 at the top to 1 at the bottom.
        // A row with rows under it is one block: a drop goes only before or after the block.
        public DropPlace? Locate(TreeNode source, TreeNode hovered, double fraction)
        {
            if (source.Item == null || source.Owner == null || hovered.IsOrIsUnder(source))
                return null;

            if (GroupUnder(source, hovered) is { } forwarded && TakesDropOf(source, forwarded))
                return new DropPlace(hovered, DropPosition.Inside);

            var target = hovered;
            while (target != null && !TakesDropOf(source, target))
                target = target.Parent;

            if (target == null)
                return null;

            if (ReferenceEquals(hovered, target))
            {
                if (target.Item == null)
                    return new DropPlace(target, DropPosition.Inside);

                if (fraction < 0.5)
                    return new DropPlace(target, DropPosition.Before);

                return ReferenceEquals(TreeRowOrder.LastVisible(target), target) ? new DropPlace(target, DropPosition.After) : null;
            }

            return ReferenceEquals(TreeRowOrder.LastVisible(target), hovered) && fraction >= 0.5 ? new DropPlace(target, DropPosition.After) : null;
        }

        private static bool TakesDropOf(TreeNode source, TreeNode target) =>
            TakesDropOfByKind(source, target) && ListOfTargetAccepts(source, target);

        // Only the kind counts here, not ListRules.CanHold.
        private static bool TakesDropOfByKind(TreeNode source, TreeNode target)
        {
            if (target.IsOrIsUnder(source))
                return false;

            // A group takes an item of another list only when it does not have the item already.
            if (target.Item == null)
                return target.ChildManager != null
                    && KindOfItem(source) is { } kind
                    && KindOfGroup(target) == kind
                    && (ReferenceEquals(target.ChildManager, source.Owner) || !target.ChildManager.ManagerData.Items.Contains(source.Item!));

            if (target.Owner == null)
                return false;

            return ReferenceEquals(source.Owner, target.Owner)
                || CanMoveBetweenListsByKind(source, target);
        }

        private static bool ListOfTargetAccepts(TreeNode source, TreeNode target) =>
            target.Item == null
                ? Accepts(source.Item!, target.Group)
                : ReferenceEquals(source.Owner, target.Owner) || Accepts(source.Item!, target.Parent?.Group);

        private static TreeNode? GroupUnder(TreeNode source, TreeNode target)
        {
            var layerOnComposition = target.Item is Composition && source.Item is Layer;
            var itemOnLayer = target.Item is Layer && source.Item is not Layer;
            if (!(layerOnComposition || itemOnLayer) || KindOfItem(source) is not { } kind)
                return null;

            return target.Children.FirstOrDefault(child => KindOfGroup(child) == kind);
        }

        public static TreeNode InsideOf(TreeNode source, TreeNode target) =>
            GroupUnder(source, target) ?? target;

        // The type of the group is the kind of its list.
        private static System.Type? KindOfItem(TreeNode item) =>
            item.Item != null ? item.Parent?.Group?.GetType() : null;

        private static System.Type? KindOfGroup(TreeNode group) =>
            group.Item == null ? group.Group?.GetType() : null;

        private static bool Accepts(IControl item, TreeGroup? group) =>
            group == null || ListRules.CanHold(group.OwnerType, item.GetType());

        private static bool CanMoveBetweenLists(TreeNode source, TreeNode target) =>
            CanMoveBetweenListsByKind(source, target) && Accepts(source.Item!, target.Parent?.Group);

        private static bool CanMoveBetweenListsByKind(TreeNode source, TreeNode target) =>
            KindOfItem(source) is { } kind
            && kind == KindOfItem(target)
            && target.Owner != null
            && !ReferenceEquals(source.Owner, target.Owner)
            && !target.Owner.ManagerData.Items.Contains(source.Item!);

        private static IReadOnlyList<Layer> LayersSharing(TreeNode source)
        {
            if (TreeRowOrder.CompositionOf(source) is not { Item: Composition composition } || source.Parent?.Parent?.Item is not Layer ownLayer)
                return Array.Empty<Layer>();

            if (source.Item == null || source.Parent?.Group is not LayerGroup)
                return Array.Empty<Layer>();

            return ownLayer.GetLayersHolding(source.Item, composition.LayerManager.ManagerData.Items.OfType<Layer>());
        }

        // A drop that shared items block is still possible, so the user can read the reason.
        public bool CanDrop(TreeNode source, TreeNode target, DropPosition position) =>
            Resolve(source, target, position) != null;

        // These rows are lit because a drop inside has no bar to show the place.
        public bool IsInsideTarget(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOf(source, InsideOf(source, target))
            && Resolve(source, target, DropPosition.Inside) != null;

        public bool IsTargetByKind(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOfByKind(source, InsideOf(source, target));

        public string? BlockReason(TreeNode source, TreeNode target)
        {
            if (source.Item == null || source.Owner == null || target.IsOrIsUnder(source) || ListOf(source, target) is not { } place)
                return null;

            var (list, group) = place;
            if (ReferenceEquals(list, source.Owner))
                return null;

            if (group != null && !ListRules.CanHold(group.OwnerType, source.Item.GetType()))
                return TreeDropText.CannotHold(source.Item, group.OwnerType);

            if (list.CanAddExistingItem?.Invoke(source.Item) == false)
                return TreeDropText.CannotTake(InsideOf(source, target), source);

            if (ReferenceEquals(TreeRowOrder.CompositionOf(source), TreeRowOrder.CompositionOf(target)))
                return null;

            if (source.Item is not Layer && LayersSharing(source) is { Count: > 0 } sharing)
                return TreeDropText.AlsoHeldBy(sharing);

            if (source.Item is IMoveRestricted restricted && restricted.GetItemsBlockingMove(source.Owner.ManagerData.Items) is { Count: > 0 } blocking)
                return TreeDropText.SharesWithOtherLayers(blocking);

            return null;
        }

        private static ListAndGroup? ListOf(TreeNode source, TreeNode target)
        {
            var inside = InsideOf(source, target);
            if (inside.Item == null)
                return inside.ChildManager == null ? null : new ListAndGroup(inside.ChildManager, inside.Group);

            return inside.Owner == null ? null : new ListAndGroup(inside.Owner, inside.Parent?.Group);
        }

        public bool IsNeighbor(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOf(source, target)
            && (Resolve(source, target, DropPosition.Before) != null || Resolve(source, target, DropPosition.After) != null);

        public DropOutcome Drop(TreeNode source, TreeNode target, DropPosition position)
        {
            var resolved = Resolve(source, target, position);
            if (resolved == null)
                return DropOutcome.None;

            var (list, index) = resolved.Value;
            var item = source.Item!;
            var owner = source.Owner!;
            if (ReferenceEquals(list, owner))
            {
                list.MoveItem(list.ManagerData.Items.IndexOf(item), NewIndex(source, list, index));
                return new DropOutcome(true, new List<IPrefab>());
            }

            // An item of a layer goes to another composition only if no other layer of its composition uses it.
            if (source.Item is not Layer && !ReferenceEquals(TreeRowOrder.CompositionOf(source), TreeRowOrder.CompositionOf(target))
                && LayersSharing(source) is { Count: > 0 } sharing)
                return new DropOutcome(false, sharing.Cast<IPrefab>().ToList());

            return DropOutcome.From(list.MoveItemToManager(item, owner, index));
        }

        public string DescribeBlock(TreeNode source, TreeNode target, IReadOnlyList<IPrefab> blocking) =>
            TreeDropText.DescribeBlock(source, target, blocking);

        private static ListPlace? Resolve(TreeNode source, TreeNode target, DropPosition position)
        {
            if (source.Item == null || source.Owner == null)
                return null;

            if (target.IsOrIsUnder(source))
                return null;

            ListPlace place;
            if (position == DropPosition.Inside)
            {
                var inside = InsideOf(source, target);
                if (inside.ChildManager == null)
                    return null;

                if (!TakesDropOf(source, inside))
                    return null;

                place = new ListPlace(inside.ChildManager, inside.ChildManager.ManagerData.Items.Count);
            }
            else
            {
                if (target.Item == null || target.Owner == null)
                    return null;

                var sameList = ReferenceEquals(source.Owner, target.Owner);
                var layerToLayerList = source.Item is Layer && target.Item is Layer;
                if (!sameList && !layerToLayerList && !CanMoveBetweenLists(source, target))
                    return null;

                var index = target.Owner.ManagerData.Items.IndexOf(target.Item) + (position == DropPosition.After ? 1 : 0);
                place = new ListPlace(target.Owner, index);
            }

            // A drop that leaves the item where it is is not a drop.
            if (ReferenceEquals(place.List, source.Owner) && NewIndex(source, place.List, place.Index) == place.List.ManagerData.Items.IndexOf(source.Item))
                return null;

            return place;
        }

        // The index is counted before the move.
        private static int NewIndex(TreeNode source, PrefabManager list, int index)
        {
            var oldIndex = list.ManagerData.Items.IndexOf(source.Item!);
            return index > oldIndex ? index - 1 : index;
        }
    }
}
