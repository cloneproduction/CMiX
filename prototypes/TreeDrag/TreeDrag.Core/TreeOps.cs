using System.Collections.Immutable;

namespace TreeDrag.Core;

/// <summary>Pure functions on the tree. They do not touch the visual tree.</summary>
public static class TreeOps
{
    /// <summary>Gives the rect of each visible node. The root is not shown, its children have depth 0.</summary>
    public static Dictionary<string, Rect> Layout(Node root, double rowHeight = 28, double indent = 20, double width = 240)
    {
        var rects = new Dictionary<string, Rect>();
        var row = 0;

        void Visit(Node node, int depth)
        {
            foreach (var child in node.Children)
            {
                var x = depth * indent;
                rects[child.Id] = new Rect(x, row * rowHeight, Math.Max(0, width - x), rowHeight);
                row++;
                if (child.IsExpanded)
                    Visit(child, depth + 1);
            }
        }

        if (root.IsExpanded)
            Visit(root, 0);
        return rects;
    }

    public static Node? Find(Node root, string id)
    {
        if (root.Id == id)
            return root;
        foreach (var child in root.Children)
        {
            var found = Find(child, id);
            if (found is not null)
                return found;
        }
        return null;
    }

    public static bool IsInSubtree(Node subtreeRoot, string id) => Find(subtreeRoot, id) is not null;

    /// <summary>Gives the tree with the expanded state of the node inverted.</summary>
    public static Node Toggle(Node root, string id) =>
        root.Id == id
            ? root with { IsExpanded = !root.IsExpanded }
            : root with { Children = root.Children.Select(c => Toggle(c, id)).ToImmutableList() };

    /// <summary>Gives the tree without the node and its subtree.</summary>
    public static Node Remove(Node root, string id) =>
        root with { Children = root.Children.Where(c => c.Id != id).Select(c => Remove(c, id)).ToImmutableList() };

    /// <summary>Gives the tree with the node inserted in the children of the parent.</summary>
    public static Node Insert(Node root, string parentId, int index, Node node)
    {
        if (root.Id == parentId)
            return root with { Children = root.Children.Insert(Math.Clamp(index, 0, root.Children.Count), node) };
        return root with { Children = root.Children.Select(c => Insert(c, parentId, index, node)).ToImmutableList() };
    }

    /// <summary>Gives the tree with the node moved. The index is in the children of the parent after the removal.</summary>
    public static Node Move(Node root, string id, string parentId, int index)
    {
        if (id == root.Id)
            throw new ArgumentException("The root cannot move.", nameof(id));
        var node = Find(root, id) ?? throw new ArgumentException($"No node {id}.", nameof(id));
        if (IsInSubtree(node, parentId))
            throw new ArgumentException("A node cannot move into its own subtree.", nameof(parentId));
        var without = Remove(root, id);
        if (Find(without, parentId) is null)
            throw new ArgumentException($"No parent {parentId}.", nameof(parentId));
        return Insert(without, parentId, index, node);
    }

    /// <summary>
    /// Gives every slot where the node can go. A parent must be visible and expanded, and not in the subtree of the node.
    /// The rule canDrop(node, parent) can reject a parent. The current slot of the node always stays legal.
    /// </summary>
    public static IEnumerable<Slot> LegalSlots(Node root, string dragId, Func<Node, Node, bool>? canDrop = null)
    {
        var node = Find(root, dragId) ?? throw new ArgumentException($"No node {dragId}.", nameof(dragId));
        var current = CurrentSlot(root, dragId);

        IEnumerable<Slot> Visit(Node parent)
        {
            if (parent.Id == dragId || !parent.IsExpanded)
                yield break;
            var allowed = canDrop?.Invoke(node, parent) ?? true;
            var count = parent.Children.Count(c => c.Id != dragId);
            for (var i = 0; i <= count; i++)
            {
                var slot = new Slot(parent.Id, i);
                if (allowed || slot == current)
                    yield return slot;
            }
            foreach (var child in parent.Children)
                foreach (var slot in Visit(child))
                    yield return slot;
        }

        return Visit(root);
    }

    /// <summary>Gives the parent and the index of the node now.</summary>
    public static Slot? CurrentSlot(Node root, string id)
    {
        var index = root.Children.FindIndex(c => c.Id == id);
        if (index >= 0)
            return new Slot(root.Id, index);
        foreach (var child in root.Children)
            if (CurrentSlot(child, id) is { } slot)
                return slot;
        return null;
    }

    /// <summary>Lays out the tree of each legal slot and takes the rect of the dragged node.</summary>
    public static List<Candidate> Candidates(Node root, string dragId, double rowHeight = 28, double indent = 20, double width = 240,
        Func<Node, Node, bool>? canDrop = null) =>
        LegalSlots(root, dragId, canDrop)
            .Select(slot =>
            {
                var tree = Move(root, dragId, slot.ParentId, slot.Index);
                return new Candidate(slot, tree, Layout(tree, rowHeight, indent, width)[dragId]);
            })
            .ToList();

    /// <summary>Gives the candidate whose spot has its top left corner nearest the point.</summary>
    public static Candidate Nearest(IReadOnlyList<Candidate> candidates, double x, double y) =>
        candidates.MinBy(c => (c.Spot.X - x) * (c.Spot.X - x) + (c.Spot.Y - y) * (c.Spot.Y - y))
        ?? throw new ArgumentException("No candidates.", nameof(candidates));
}
