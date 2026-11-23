using TreeDrag.Core;
using static TreeDrag.Core.TreeOps;

namespace TreeDrag.Core.Tests;

public class TreeOpsTests
{
    // root: a (a1, a2), b, c collapsed (c1)
    private static Node Sample() => Node.Branch("root",
        Node.Branch("a", Node.Leaf("a1"), Node.Leaf("a2")),
        Node.Leaf("b"),
        Node.Branch("c", Node.Leaf("c1")) with { IsExpanded = false });

    private static string Shape(Node node) =>
        node.Children.Count == 0 ? node.Id : $"{node.Id}({string.Join(",", node.Children.Select(Shape))})";

    [Fact]
    public void Layout_Sample_IndentsByDepthAndStacksRows()
    {
        var rects = Layout(Sample());

        Assert.Equal(new Rect(0, 0, 240, 28), rects["a"]);
        Assert.Equal(new Rect(20, 28, 220, 28), rects["a1"]);
        Assert.Equal(new Rect(20, 56, 220, 28), rects["a2"]);
        Assert.Equal(new Rect(0, 84, 240, 28), rects["b"]);
        Assert.Equal(new Rect(0, 112, 240, 28), rects["c"]);
    }

    [Fact]
    public void Layout_CollapsedNode_SkipsItsChildren()
    {
        var rects = Layout(Sample());

        Assert.False(rects.ContainsKey("c1"));
        Assert.False(rects.ContainsKey("root"));
        Assert.Equal(5, rects.Count);
    }

    [Fact]
    public void Move_ToOtherParent_Reparents()
    {
        Assert.Equal("root(a(a1),b(a2),c(c1))", Shape(Move(Sample(), "a2", "b", 0)));
    }

    [Fact]
    public void Move_InSameParent_Reorders()
    {
        Assert.Equal("root(b,a(a1,a2),c(c1))", Shape(Move(Sample(), "a", "root", 1)));
        Assert.Equal("root(a(a2,a1),b,c(c1))", Shape(Move(Sample(), "a1", "a", 1)));
    }

    [Fact]
    public void Move_IntoOwnSubtree_Throws()
    {
        Assert.Throws<ArgumentException>(() => Move(Sample(), "a", "a1", 0));
        Assert.Throws<ArgumentException>(() => Move(Sample(), "a", "a", 0));
    }

    [Fact]
    public void Move_Root_Throws()
    {
        Assert.Throws<ArgumentException>(() => Move(Sample(), "root", "a", 0));
    }

    [Fact]
    public void LegalSlots_DragA_SkipsOwnSubtreeAndCollapsedNodes()
    {
        var slots = LegalSlots(Sample(), "a").ToList();

        // root keeps b and c, so 3 slots. b has 1 slot. a, a1, a2 and the collapsed c have none.
        Assert.Equal(new[] { new Slot("root", 0), new Slot("root", 1), new Slot("root", 2), new Slot("b", 0) }, slots);
    }

    [Fact]
    public void LegalSlots_DragLeaf_IncludesTheSlotWhereItIs()
    {
        Assert.Contains(new Slot("a", 1), LegalSlots(Sample(), "a2"));
    }

    [Fact]
    public void Candidates_DragA2_GivesTheSpotOfEachResultTree()
    {
        var candidates = Candidates(Sample(), "a2");

        var intoB = candidates.Single(c => c.Slot == new Slot("b", 0));
        Assert.Equal(new Rect(20, 84, 220, 28), intoB.Spot);
        var top = candidates.Single(c => c.Slot == new Slot("root", 0));
        Assert.Equal(new Rect(0, 0, 240, 28), top.Spot);
    }

    [Fact]
    public void Nearest_PointNearSpot_PicksThatCandidate()
    {
        var candidates = Candidates(Sample(), "a2");

        Assert.Equal(new Slot("b", 0), Nearest(candidates, 22, 86).Slot);
        Assert.Equal(new Slot("root", 0), Nearest(candidates, -5, -10).Slot);
    }

    [Fact]
    public void Toggle_CollapsedNode_ShowsItsChildren()
    {
        var rects = Layout(Toggle(Sample(), "c"));

        Assert.Equal(new Rect(20, 140, 220, 28), rects["c1"]);
    }

    [Fact]
    public void Toggle_ExpandedNode_HidesItsChildrenAndMovesTheRowsBelow()
    {
        var rects = Layout(Toggle(Sample(), "a"));

        Assert.False(rects.ContainsKey("a1"));
        Assert.Equal(new Rect(0, 28, 240, 28), rects["b"]);
    }

    [Fact]
    public void Toggle_Twice_GivesTheSameShapeAndState()
    {
        var tree = Toggle(Toggle(Sample(), "a"), "a");

        Assert.True(Find(tree, "a")!.IsExpanded);
        Assert.Equal(Shape(Sample()), Shape(tree));
    }

    private static bool OnlyIntoGroups(Node node, Node parent) => parent.Id == "root" || parent.Children.Count > 0;

    [Fact]
    public void LegalSlots_RuleRejectsLeaves_SkipsTheLeafParents()
    {
        var slots = LegalSlots(Sample(), "a2", OnlyIntoGroups).ToList();

        Assert.DoesNotContain(slots, s => s.ParentId == "b");
        Assert.Contains(new Slot("a", 0), slots);
        Assert.Contains(new Slot("root", 3), slots);
    }

    [Fact]
    public void LegalSlots_RuleRejectsAll_KeepsOnlyTheCurrentSlot()
    {
        Assert.Equal(new[] { new Slot("a", 1) }, LegalSlots(Sample(), "a2", (_, _) => false));
    }

    [Fact]
    public void LegalSlots_Rule_GetsTheDraggedNodeAndTheParent()
    {
        var calls = new List<(string, string)>();
        LegalSlots(Sample(), "b", (n, p) => { calls.Add((n.Id, p.Id)); return true; }).ToList();

        Assert.All(calls, c => Assert.Equal("b", c.Item1));
        Assert.Contains(("b", "root"), calls);
        Assert.Contains(("b", "a1"), calls);
    }

    [Fact]
    public void CurrentSlot_Node_GivesItsParentAndIndex()
    {
        Assert.Equal(new Slot("a", 1), CurrentSlot(Sample(), "a2"));
        Assert.Equal(new Slot("root", 2), CurrentSlot(Sample(), "c"));
        Assert.Null(CurrentSlot(Sample(), "root"));
    }

    [Fact]
    public void Candidates_WithRule_UseOnlyTheAllowedParents()
    {
        var candidates = Candidates(Sample(), "a2", canDrop: OnlyIntoGroups);

        Assert.DoesNotContain(candidates, c => c.Slot.ParentId == "b");
    }
}
