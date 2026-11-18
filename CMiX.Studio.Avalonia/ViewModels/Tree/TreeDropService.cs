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

    // The row that takes a drop, and where the drop goes in relation to it.
    public sealed record DropPlace(TreeNode Target, DropPosition Position);

    // What a drop did: the item moved, items blocked the move (Blocking gives them), or nothing happened (None).
    public sealed record DropOutcome(bool Moved, IReadOnlyList<IPrefab> Blocking)
    {
        public static DropOutcome None { get; } = new(false, Array.Empty<IPrefab>());

        public static DropOutcome From(MoveResult result) =>
            new(result.Moved, result.Blocking);
    }

    // The rules for a drag in the project tree.
    // An item moves inside its own list, so a composition goes with compositions and an entity with entities.
    // It can also go to a list of the same kind of another owner: an entity to another layer, a modifier to another entity,
    // a filter, a texture or a modulator to another owner of the same kind of list.
    // A layer can also go to the layer list of another composition, if no item of it is shared with a layer that stays.
    // A layer row passes an item that is dropped on it to the group of the item's kind under it, and a composition row passes a layer to its Layers
    // group (see InsideOf). The compositions are the items of the Compositions group, and the layers are the items of a Layers group.
    public sealed class TreeDropService
    {
        // Where a drop lands when the pointer is over a row. The fraction is the height of the pointer on that row,
        // from 0 at the top to 1 at the bottom. The result is null when no drop is offered at this place.
        //
        // The row that takes the drop is the hovered row, or the first parent row that can take the dragged node.
        // A row with rows under it is one block, so the drop is offered only at the two ends of the block:
        // before it, in the top half of its own row, and after it, in the bottom half of the last visible row of the block.
        // A row without rows under it has both halves. A layer over a composition goes inside it, wherever the pointer is on the row.
        public DropPlace? Locate(TreeNode source, TreeNode hovered, double fraction)
        {
            if (source.Item == null || source.Owner == null || hovered.IsOrIsUnder(source))
                return null;

            // A layer row takes an item of its groups inside it, wherever the pointer is on the row.
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

        // True when the node is a row that the dragged node can go before, after, or (a layer on a composition) inside.
        private static bool TakesDropOf(TreeNode source, TreeNode target) =>
            TakesDropOfByKind(source, target) && ListOfTargetAccepts(source, target);

        // As TakesDropOf, but only the kind counts: ListRules.CanHold does not.
        private static bool TakesDropOfByKind(TreeNode source, TreeNode target)
        {
            if (target.IsOrIsUnder(source))
                return false;

            // A group takes an item of the same kind, inside it. An item of its own list goes to the end of the list. An item of another
            // list goes there too, unless that list has the item already.
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

        // The list that takes the drop holds the kind of the item. A move in its own list needs no check.
        private static bool ListOfTargetAccepts(TreeNode source, TreeNode target) =>
            target.Item == null
                ? Accepts(source.Item!, target.Group)
                : ReferenceEquals(source.Owner, target.Owner) || Accepts(source.Item!, target.Parent?.Group);

        // The group of the kind of the dragged item under a layer row, or under a composition row for a layer. Null for another row, or when the
        // row has no such group.
        private static TreeNode? GroupUnder(TreeNode source, TreeNode target)
        {
            var layerOnComposition = target.Item is Composition && source.Item is Layer;
            var itemOnLayer = target.Item is Layer && source.Item is not Layer;
            if (!(layerOnComposition || itemOnLayer) || KindOfItem(source) is not { } kind)
                return null;

            return target.Children.FirstOrDefault(child => KindOfGroup(child) == kind);
        }

        // The row whose list takes an item dropped inside the target: the group under a layer row, else the target.
        public static TreeNode InsideOf(TreeNode source, TreeNode target) =>
            GroupUnder(source, target) ?? target;

        // The kind of a list is the type of its group: entities, cameras, lights, textures, filters, modifiers or modulators.
        // An item moves between the lists of two owners only into a list of its own kind.
        private static System.Type? KindOfItem(TreeNode item) =>
            item.Item != null ? item.Parent?.Group?.GetType() : null;

        private static System.Type? KindOfGroup(TreeNode group) =>
            group.Item == null ? group.Group?.GetType() : null;

        // The list of the group can hold the item. ListRules.CanHold decides.
        private static bool Accepts(IControl item, TreeGroup? group) =>
            group == null || ListRules.CanHold(group.OwnerType, item.GetType());

        // The item is in a list of the same kind as the row of another owner, and that list does not have it yet.
        private static bool CanMoveBetweenLists(TreeNode source, TreeNode target) =>
            CanMoveBetweenListsByKind(source, target) && Accepts(source.Item!, target.Parent?.Group);

        private static bool CanMoveBetweenListsByKind(TreeNode source, TreeNode target) =>
            KindOfItem(source) is { } kind
            && kind == KindOfItem(target)
            && target.Owner != null
            && !ReferenceEquals(source.Owner, target.Owner)
            && !target.Owner.ManagerData.Items.Contains(source.Item!);

        // The other layers of the composition that hold the item of the source. Empty when the source is not in an entity, camera or light list.
        private static IReadOnlyList<Layer> LayersSharing(TreeNode source)
        {
            if (TreeRowOrder.CompositionOf(source) is not { Item: Composition composition } || source.Parent?.Parent?.Item is not Layer ownLayer)
                return Array.Empty<Layer>();

            if (source.Item == null || source.Parent?.Group is not LayerGroup)
                return Array.Empty<Layer>();

            return ownLayer.GetLayersHolding(source.Item, composition.LayerManager.ManagerData.Items.OfType<Layer>());
        }

        // True when the drop is possible by type and place. A move that shared items block is still possible here,
        // so the user can drop it and read the reason.
        public bool CanDrop(TreeNode source, TreeNode target, DropPosition position) =>
            Resolve(source, target, position) != null;

        // True when the dragged node can go inside the row: a group takes an item of its kind, and a composition takes a layer.
        // A drag shows these rows, since the drop on them has no bar between two rows to tell the place.
        public bool IsInsideTarget(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOf(source, InsideOf(source, target))
            && Resolve(source, target, DropPosition.Inside) != null;

        // True when the row is a target by the kind of the dragged node alone, before the rules of the list (see BlockReason).
        public bool IsTargetByKind(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOfByKind(source, InsideOf(source, target));

        // The reason why a drop on the row does not go through, in one line, or null when it goes through.
        // The list of the row (inside it, or next to it) must hold the kind of the item (ListRules.CanHold) and take it
        // (CanAddExistingItem). An entity, a camera or a light cannot go to another composition while another layer holds it,
        // and a layer cannot go there while it shares items with another layer.
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

        // The list that takes an item dropped on the row, and its group: inside a group, a layer row or a composition, else next to the row.
        private static ListAndGroup? ListOf(TreeNode source, TreeNode target)
        {
            var inside = InsideOf(source, target);
            if (inside.Item == null)
                return inside.ChildManager == null ? null : new ListAndGroup(inside.ChildManager, inside.Group);

            return inside.Owner == null ? null : new ListAndGroup(inside.Owner, inside.Parent?.Group);
        }

        // True when the dragged node can go before or after the row, in the list of the row. The place is shown by a bar between two rows.
        public bool IsNeighbor(TreeNode source, TreeNode target) =>
            source.Item != null && source.Owner != null
            && TakesDropOf(source, target)
            && (Resolve(source, target, DropPosition.Before) != null || Resolve(source, target, DropPosition.After) != null);

        public DropOutcome Drop(TreeNode source, TreeNode target, DropPosition position)
        {
            var resolved = Resolve(source, target, position);
            if (resolved == null)
                return DropOutcome.None;

            var (list, index) = resolved;
            var item = source.Item!;
            var owner = source.Owner!;
            if (ReferenceEquals(list, owner))
            {
                list.MoveItem(list.ManagerData.Items.IndexOf(item), NewIndex(source, list, index));
                return new DropOutcome(true, new List<IPrefab>());
            }

            // An item of a layer can go to a layer of another composition only if no other layer of its composition uses it.
            if (source.Item is not Layer && !ReferenceEquals(TreeRowOrder.CompositionOf(source), TreeRowOrder.CompositionOf(target))
                && LayersSharing(source) is { Count: > 0 } sharing)
                return new DropOutcome(false, sharing.Cast<IPrefab>().ToList());

            return DropOutcome.From(list.MoveItemToManager(item, owner, index));
        }

        // The text for a move that shared items block. For an entity, a camera or a light, blocking gives the layers that share it.
        public string DescribeBlock(TreeNode source, TreeNode target, IReadOnlyList<IPrefab> blocking) =>
            TreeDropText.DescribeBlock(source, target, blocking);

        // The list that takes the item, and the index in it. Null when the drop is not possible.
        private static ListPlace? Resolve(TreeNode source, TreeNode target, DropPosition position)
        {
            // A group and the top of the tree cannot be dragged.
            if (source.Item == null || source.Owner == null)
                return null;

            if (target.IsOrIsUnder(source))
                return null;

            ListPlace place;
            if (position == DropPosition.Inside)
            {
                // A composition takes a layer inside it, and a group takes an item of its kind, at the end. A layer row passes the item to its group.
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

            // A drop that leaves the item where it is changes nothing, so it is not a drop.
            if (ReferenceEquals(place.List, source.Owner) && NewIndex(source, place.List, place.Index) == place.List.ManagerData.Items.IndexOf(source.Item))
                return null;

            return place;
        }

        // The index of the item in its own list after it moves to the place with that index, counted before the move.
        private static int NewIndex(TreeNode source, PrefabManager list, int index)
        {
            var oldIndex = list.ManagerData.Items.IndexOf(source.Item!);
            return index > oldIndex ? index - 1 : index;
        }
    }
}
