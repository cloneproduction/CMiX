// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // One drag of a row of the new tree: the candidates of the live tree and the one that the pointer picks.
    public sealed class ListTreeDrag
    {
        private readonly ListTreeCandidates candidates;
        private readonly ListTreePick pick;

        private ListTreeSnapshot live = new(Array.Empty<ListTreeRow>());
        private ListTreeRow? source;
        private IReadOnlyList<ListTreeCandidate> made = Array.Empty<ListTreeCandidate>();
        private ListTreeLight? light;

        public ListTreeDrag(ListTreeCandidates candidates, ListTreePick pick, TreeNode node, ListTreePress press)
        {
            this.candidates = candidates;
            this.pick = pick;
            Node = node;
            Press = press;
        }

        public TreeNode Node { get; }

        public ListTreePress Press { get; }

        public IReadOnlyList<ListTreeCandidate> Candidates => made;

        // Null before the first pick, and when no place takes the row.
        public ListTreeDragLook? Look { get; private set; }

        // A card that opens during the drag has new places, so the candidates are made again.
        public void MakeCandidates(ListTreeSnapshot snapshot)
        {
            live = snapshot;
            source = snapshot.RowOf(Node);
            made = source != null ? candidates.Of(snapshot, source) : Array.Empty<ListTreeCandidate>();
            light = source != null ? ListTreeBrightRows.Of(snapshot, source, made) : null;
        }

        // The look stays the same object while the pick does not change.
        public ListTreeDragLook? PickAt(ListTreePoint pointer)
        {
            if (source == null || light == null || made.Count == 0)
                return Look = null;

            var picked = Look is { } shown ? pick.NearestOrCurrent(made, shown.Candidate, Press, pointer) : pick.Nearest(made, Press, pointer);
            if (!ReferenceEquals(picked, Look?.Candidate))
                Look = ListTreeDragLook.Of(live, source, picked, light);

            return Look;
        }

        // The slot of the row itself: a drop there moves nothing.
        public bool IsAtItsOwnPlace() => Look is { } look && ReferenceEquals(look.Candidate.Slot.Target, Node);
    }
}
