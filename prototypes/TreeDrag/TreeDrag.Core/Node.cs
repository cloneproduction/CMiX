using System.Collections.Immutable;

namespace TreeDrag.Core;

/// <summary>An immutable tree node. The parent and child links exist only here.</summary>
public sealed record Node(string Id, ImmutableList<Node> Children, bool IsExpanded = true)
{
    public static Node Leaf(string id) => new(id, ImmutableList<Node>.Empty);

    public static Node Branch(string id, params Node[] children) => new(id, children.ToImmutableList());
}

/// <summary>A rectangle in panel coordinates.</summary>
public readonly record struct Rect(double X, double Y, double Width, double Height);

/// <summary>A place where a node can go: the parent and the index in its children.</summary>
public readonly record struct Slot(string ParentId, int Index);

/// <summary>A possible result of a drag: the tree after the move and the rect of the dragged node in it.</summary>
public sealed record Candidate(Slot Slot, Node Tree, Rect Spot);
