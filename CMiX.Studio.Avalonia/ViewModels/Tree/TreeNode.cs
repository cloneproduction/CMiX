// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // A node of the project tree: a composition, a layer, an item of a layer, or a group of items.
    // Item is null for a group. Owner is the list that holds the Item.
    // ChildManager is the list whose items are the children. The children follow that list.
    public sealed partial class TreeNode : ObservableObject, IDisposable
    {
        private readonly ProjectTree tree;

        internal TreeNode(ProjectTree tree, TreeNode? parent, PrefabManager? owner, IControl? item, string header, PrefabManager? childManager, TreeGroup? group = null)
        {
            this.tree = tree;

            // A row made while the project loads starts collapsed. Any other new row starts expanded.
            isExpanded = !tree.IsLoading;
            Parent = parent;
            Owner = owner;
            Item = item;
            Group = group;
            this.header = group?.Header ?? header;
            IconKey = group?.IconKey ?? IconFor(item);

            if (item is IPrefab prefab)
            {
                this.header = prefab.PrefabService.Name.Value;
                prefab.PrefabService.Name.PropertyChanged += OnNameChanged;
            }

            Category = item != null ? TreeCategories.Of(item) : group?.Category;
            kindLabel = item != null ? AddableTypes.LabelOf(item.GetType()) : null;

            // A row tells its rows below when it is hidden or shown, so they look greyed (see IsUnderHiddenParent).
            hiddenSource = HiddenSourceOf(group, item);
            if (hiddenSource != null)
                hiddenSource.PropertyChanged += OnHiddenChanged;
            // A group of composables (Compositions, Layers) takes no level of its own: the compositions and the layers have the same colors as before.
            ComposableDepth = parent == null ? 0 : parent.ComposableDepth + (parent.Category == TreeCategory.Composable && parent.Item != null ? 1 : 0);
            KindDepth = parent == null ? 0 : parent.KindDepth + (parent.Category != null && Category != null && parent.Category != Category ? 1 : 0);

            ChildManager = childManager ?? (group?.Manager);
            if (ChildManager != null)
            {
                foreach (var child in ChildManager.ManagerData.Items)
                    Children.Add(CreateItemNode(child));

                ChildManager.ManagerData.Items.CollectionChanged += OnItemsChanged;
                ChildManager.ManagerData.PropertyChanged += OnManagerDataChanged;
            }

            // The groups come after the items of the node (the layers of a composition), so the place of an item in its list is its
            // place in the children.
            if (item != null)
                AddGroups(item);

            // The counts are computed once here, and again only when the children change.
            kindCounts = ComputeKindCounts();
            hiddenCount = ComputeHiddenCount();
            missingCount = ComputeMissingCount();
            WatchTheAsset();
            Children.CollectionChanged += (_, _) =>
            {
                RaiseKindCountsChanged();
                RecountHidden();
                RecountMissing();
            };
        }

        // The lists that an item holds, as groups (see TreeGroup.GroupsOf). A group row has no item. The inspector shows the panel of the group.
        internal void AddGroups(IControl item)
        {
            foreach (var group in TreeGroup.GroupsOf(item))
                Children.Add(new TreeNode(tree, this, null, null, group.Header, group.Manager, group));
        }

        public TreeNode? Parent { get; }
        public string IconKey { get; }
        public IControl? Item { get; }
        public TreeGroup? Group { get; }

        // The kind of the row: its color in the tree. Null for a row that the tree does not know.
        public TreeCategory? Category { get; }

        // The number of composables around the row. A composable has one more color for each level.
        public int ComposableDepth { get; }

        // The number of changes of kind from the top of the tree to the row. A group and its items have the same one, and what a row holds
        // of another kind has one more. A row is darker for each one (see CategoryPalette).
        public int KindDepth { get; }

        // What the inspector shows for this row: the item, or the group of items of a layer.
        public object? InspectorContent => (object?)Item ?? Group;
        public PrefabManager? Owner { get; }
        public PrefabManager? ChildManager { get; }
        public ObservableCollection<TreeNode> Children { get; } = new();

        // The kinds of what the row holds, in the order of the rows under it, each with its number: an item under the row counts for
        // its own kind, and a group under the row counts for the items in it. A collapsed row shows these.
        public IReadOnlyList<KindCount> KindCounts => kindCounts;

        private IReadOnlyList<KindCount> kindCounts;

        // The number of computations of the counts. A test reads it.
        internal int KindCountsComputations { get; private set; }

        private List<KindCount> ComputeKindCounts()
        {
            KindCountsComputations++;
            var counts = new Dictionary<TreeCategory, int>();
            foreach (var child in Children)
            {
                if (child.Category is not { } category)
                    continue;

                var number = child.Item != null ? 1 : child.Children.Count;
                if (number > 0)
                    counts[category] = counts.GetValueOrDefault(category) + number;
            }

            return counts.Select(count => new KindCount(count.Key, count.Value)).ToList();
        }

        public bool HasKindCounts => kindCounts.Count > 0;

        // A collapsed row shows at most this number of kind squares. One more square shows the number of the other kinds.
        public const int MaxShownKindCounts = 3;

        public IReadOnlyList<KindCount> ShownKindCounts => kindCounts.Count <= MaxShownKindCounts ? kindCounts : kindCounts.Take(MaxShownKindCounts).ToList();

        // The number of kinds that the row does not show as squares.
        public int MoreKindsCount => Math.Max(0, kindCounts.Count - MaxShownKindCounts);

        public bool HasMoreKinds => MoreKindsCount > 0;

        // The tooltip of the square of the other kinds: one kind on each line.
        public string MoreKindsDescription => string.Join(Environment.NewLine, kindCounts.Skip(MaxShownKindCounts).Select(count => count.Description));

        // The item that has a mask: a composition or a layer. The other rows have none.
        public IControl? MaskOwner => Item is Composition or Layer ? Item : null;

        // A collapsed row shows what it holds, and how many hidden rows it holds. An open row shows it in the rows under it.
        public bool ShowsKindCounts => (HasKindCounts || HasHiddenRows) && !IsExpanded;

        // The sign of a missing asset follows the rule of the kind squares: a collapsed row shows it for the rows under it, an open row
        // leaves it to them. The missing row itself always shows it, so the sign is seen at every level down to the file.
        public bool ShowsMissingSign => HasMissingAssets && (IsMissingAsset || !IsExpanded);

        // The badges of a row: the kind squares of a collapsed row, or the sign of a missing asset (see KindCountBadges).
        public bool ShowsBadges => ShowsKindCounts || ShowsMissingSign;

        // A missing asset: the item is a texture source with a file that is not on disk. A path with no asset counts as missing too.
        public bool IsMissingAsset =>
            Item is IAssetTextureSource { AssetSelector: { } selector }
            && (selector.Asset is { FileExist: false } || (selector.Asset == null && !string.IsNullOrEmpty(selector.FilePath?.Value)));

        // The missing assets on this row and under it. The row above counts them too, so the count bubbles up to the composition.
        public int MissingCount => missingCount;

        public bool HasMissingAssets => missingCount > 0;

        private int missingCount;

        private int ComputeMissingCount() => (IsMissingAsset ? 1 : 0) + Children.Sum(child => child.MissingCount);

        // Computes the count again. When it changes, the row above computes its count again too.
        private void RecountMissing()
        {
            var count = ComputeMissingCount();
            if (count == missingCount)
                return;

            missingCount = count;
            OnPropertyChanged(nameof(MissingCount));
            OnPropertyChanged(nameof(HasMissingAssets));
            OnPropertyChanged(nameof(ShowsMissingSign));
            OnPropertyChanged(nameof(ShowsBadges));
            Parent?.RecountMissing();
        }

        // The asset of a texture source can change or come back: the count follows the selector.
        private void WatchTheAsset()
        {
            if (Item is IAssetTextureSource { AssetSelector: { } selector })
                selector.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(AssetSelector.Asset))
                        RecountMissing();
                };
        }

        // The rows under a group change what its parent holds, so the parent tells its own change too.
        private void RaiseKindCountsChanged()
        {
            kindCounts = ComputeKindCounts();
            OnPropertyChanged(nameof(KindCounts));
            OnPropertyChanged(nameof(HasKindCounts));
            OnPropertyChanged(nameof(ShownKindCounts));
            OnPropertyChanged(nameof(MoreKindsCount));
            OnPropertyChanged(nameof(HasMoreKinds));
            OnPropertyChanged(nameof(MoreKindsDescription));
            OnPropertyChanged(nameof(ShowsKindCounts));
            OnPropertyChanged(nameof(ShowsBadges));
            if (Item == null)
                Parent?.RaiseKindCountsChanged();
        }

        [ObservableProperty]
        private string header;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowsKindCounts))]
        [NotifyPropertyChangedFor(nameof(ShowsMissingSign))]
        [NotifyPropertyChangedFor(nameof(ShowsBadges))]
        [NotifyPropertyChangedFor(nameof(IsDropHint))]
        [NotifyPropertyChangedFor(nameof(IsDimmed))]
        private bool isExpanded = true;

        // True for a row of an item that was added after the tree was made, and not by the load of a project. The row fades in.
        public bool IsFresh { get; private set; }

        // True when the row is the cursor row (ProjectTree.CursorNode). The row theme binds CursorRow.IsCursor to it.
        [ObservableProperty]
        private bool isCursor;

        // True when the row is above the cursor row: it is drawn over the rows around it, with the cursor row (see TreeLiftConverter).
        [ObservableProperty]
        private bool isOnCursorBranch;

        // Sets IsOnCursorBranch on each row above the cursor row.
        internal static void SetCursorBranch(TreeNode? cursor, bool value)
        {
            for (var parent = cursor?.Parent; parent != null; parent = parent.Parent)
                parent.IsOnCursorBranch = value;
        }

        // The state of the row during a drag of a node (see ProjectTree.BeginDrag). The rows that take the node inside are candidates: they
        // are lit. The rows that the node can go next to are neighbors: they stay as they are, and the bar shows the place. A collapsed row
        // that has such a row under it gives a hint, so the user knows to open it. A row that takes the node by its kind but refuses
        // it by a rule is blocked: it keeps its fill, shows a frame and gives the reason. The other rows are dimmed.
        private bool isDragActive;
        private bool isDragSource;
        private bool isDropCandidate;
        private bool isDropNeighbor;
        private bool hasDropTargetBelow;
        private string? dropBlockReason;

        public bool IsDragActive { get => isDragActive; private set => SetProperty(ref isDragActive, value); }
        public bool IsDragSource { get => isDragSource; private set => SetProperty(ref isDragSource, value); }
        public bool IsDropCandidate { get => isDropCandidate; private set => SetProperty(ref isDropCandidate, value); }
        public bool IsDropNeighbor { get => isDropNeighbor; private set => SetProperty(ref isDropNeighbor, value); }
        public bool HasDropTargetBelow { get => hasDropTargetBelow; private set => SetProperty(ref hasDropTargetBelow, value); }

        // The reason why the row refuses the dragged node (see TreeDropService.BlockReason), or null.
        public string? DropBlockReason { get => dropBlockReason; private set => SetProperty(ref dropBlockReason, value); }
        public bool IsDropBlocked => DropBlockReason != null;
        public bool IsDropHint => IsDragActive && HasDropTargetBelow && !IsDropCandidate && !IsDropNeighbor && !IsDropBlocked && !IsExpanded;
        // Covered during a drag: every row that can not take the node, the source and a row that refuses it by a rule too (the reason
        // stays in the tooltip). A row that can take it is lit; a collapsed row with a target deeper is half covered (IsDropHint).
        public bool IsDimmed => IsDragActive && !IsDropCandidate && !IsDropNeighbor && !IsDropHint;

        internal void ShowDragLook(DragLook look)
        {
            var before = CaptureDragCover();
            IsDragActive = true;
            ApplyDragLook(look);
            RaiseDragCoverChanges(before);
        }

        internal void ClearDragLook()
        {
            var before = CaptureDragCover();
            IsDragActive = false;
            ApplyDragLook(DragLook.None);
            RaiseDragCoverChanges(before);
        }

        private void ApplyDragLook(DragLook look)
        {
            IsDragSource = look.IsSource;
            IsDropCandidate = look.IsCandidate;
            IsDropNeighbor = look.IsNeighbor;
            HasDropTargetBelow = look.HasTargetBelow;
            DropBlockReason = look.BlockReason;
        }

        private sealed record DragCover(bool IsDropBlocked, bool IsDropHint, bool IsDimmed);

        private DragCover CaptureDragCover() => new(IsDropBlocked, IsDropHint, IsDimmed);

        private void RaiseDragCoverChanges(DragCover before)
        {
            if (before.IsDropBlocked != IsDropBlocked)
                OnPropertyChanged(nameof(IsDropBlocked));

            if (before.IsDropHint != IsDropHint)
                OnPropertyChanged(nameof(IsDropHint));

            if (before.IsDimmed != IsDimmed)
                OnPropertyChanged(nameof(IsDimmed));
        }

        // The name of the type of the item, as the Add panel shows it (see AddableTypes.LabelOf), so a renamed item is found by its type.
        // Null for a group row.
        private readonly string? kindLabel;

        // The state of the row in the search of the tree (see ProjectTree.SearchText). A row that matches keeps its color. While a search
        // runs, every other row is dimmed, the parents of the matches too.
        public bool IsSearchMatch { get; private set; }
        public bool IsSearchDimmed { get; private set; }

        // True when the name of the row or the kind of its item holds the text, with no regard to case. A group row has only its name.
        internal bool MatchesSearch(string text) =>
            Header.Contains(text, StringComparison.OrdinalIgnoreCase)
            || (kindLabel != null && kindLabel.Contains(text, StringComparison.OrdinalIgnoreCase));

        // Tells only the properties whose value changes.
        internal void SetSearchState(bool active, bool match)
        {
            if (IsSearchMatch != match)
            {
                IsSearchMatch = match;
                OnPropertyChanged(nameof(IsSearchMatch));
            }

            var dimmed = active && !match;
            if (IsSearchDimmed != dimmed)
            {
                IsSearchDimmed = dimmed;
                OnPropertyChanged(nameof(IsSearchDimmed));
            }
        }

        // A new name can make the row match the search, or stop it.
        partial void OnHeaderChanged(string value) => tree.RefreshSearch();

        // The search does not close again a row that the user opened or closed. A row that closes over the cursor takes the cursor
        // (see ProjectTree.OnRowExpandedChanged).
        partial void OnIsExpandedChanged(bool value) => tree.OnRowExpandedChanged(this);

        public bool IsOrIsUnder(TreeNode node)
        {
            for (var current = this; current != null; current = current.Parent)
                if (ReferenceEquals(current, node))
                    return true;
            return false;
        }

        // The icon of an item row, or of a row that is neither an item nor a group.
        // The icon of an item: the icon of its type when it has one (see TreeIcons), else the icon of its kind.
        private static string IconFor(IControl? item) =>
            (item == null ? null : TreeIcons.OfType(item.GetType())) ?? item switch
            {
                IModifier => "Modifier",
                IModulator => "Modulator",
                ITextureFilter => "Filter",
                ITextureSource => "Texture",
                _ => "Geometry"
            };

        private TreeNode CreateItemNode(IControl item) =>
            new(tree, this, ChildManager, item, string.Empty, null);

        private void OnNameChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value" && Item is IPrefab prefab)
                Header = prefab.PrefabService.Name.Value;
        }

        private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    InsertChildren(e.NewStartingIndex, e.NewItems!.Cast<IControl>(), fresh: !tree.IsLoading);

                    // A new item shows at once: the rows above it open. A loaded item opens nothing.
                    if (!tree.IsLoading)
                        for (var holder = this; holder != null; holder = holder.Parent)
                            holder.IsExpanded = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveChildren(e.OldStartingIndex, e.OldItems!.Count);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Children.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveChildren(e.OldStartingIndex, e.OldItems!.Count);
                    InsertChildren(e.NewStartingIndex, e.NewItems!.Cast<IControl>());
                    break;
                default:
                    // The groups after the items stay.
                    RemoveChildren(0, Children.Count(child => child.Item != null));
                    InsertChildren(0, ChildManager!.ManagerData.Items);
                    break;
            }

            tree.ListChanged();
        }

        private void InsertChildren(int index, IEnumerable<IControl> items, bool fresh = false)
        {
            foreach (var item in items)
            {
                var node = CreateItemNode(item);
                node.IsFresh = fresh;
                Children.Insert(index++, node);
            }
        }

        private void RemoveChildren(int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var node = Children[index];
                tree.NodeRemoved(node);
                Children.RemoveAt(index);
                node.Dispose();
            }
        }

        private void OnManagerDataChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ManagerData.SelectedItem))
                return;

            var selected = ChildManager!.ManagerData.SelectedItem;
            var node = Children.FirstOrDefault(child => ReferenceEquals(child.Item, selected));
            if (node != null)
                tree.SelectFromModel(node);
        }

        // The hide state of a row: the whole list of a group (its Hidden value), or the visibility of an item. A row that has none is never hidden.
        // The engine draws nothing of what is under a hidden row, but the rows under it keep their own state.
        private readonly INotifyPropertyChanged? hiddenSource;

        private static INotifyPropertyChanged? HiddenSourceOf(TreeGroup? group, IControl? item) =>
            group != null ? group.Manager.Hidden : (item as IPrefab)?.PrefabService.Visibility;

        public bool IsHidden => hiddenSource switch
        {
            GenericValue<bool> value => value.Value,
            _ => false
        };

        // The value that the eye toggle of the row writes: true hides the row. Null for a row that has no visibility.
        public GenericValue<bool>? HiddenValue => hiddenSource as GenericValue<bool>;

        // True when any row above this one is hidden: a hidden list, or a hidden item such as a layer or a composition. The visibility toggles of the
        // row look greyed then, as if they were disabled, but they work: the row keeps its own visibility, and it shows again with its parents.
        public bool IsUnderHiddenParent => TreeWalk.ParentsOf(this).Any(parent => parent.IsHidden);

        private void OnHiddenChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
                return;

            OnPropertyChanged(nameof(IsHidden));
            RaiseUnderHiddenParentChanged();
            Parent?.RecountHidden();
        }

        // The number of hidden rows under the row, at all levels. A collapsed row shows it, so a hidden row is never out of sight.
        public int HiddenCount => hiddenCount;

        public bool HasHiddenRows => hiddenCount > 0;

        private int hiddenCount;

        private int ComputeHiddenCount() => Children.Sum(child => (child.IsHidden ? 1 : 0) + child.HiddenCount);

        // Computes the count again. When it changes, the row above computes its count again too.
        private void RecountHidden()
        {
            var count = ComputeHiddenCount();
            if (count == hiddenCount)
                return;

            var had = HasHiddenRows;
            hiddenCount = count;
            OnPropertyChanged(nameof(HiddenCount));
            if (had != HasHiddenRows)
            {
                OnPropertyChanged(nameof(HasHiddenRows));
                OnPropertyChanged(nameof(ShowsKindCounts));
                OnPropertyChanged(nameof(ShowsBadges));
                }

            Parent?.RecountHidden();
        }

        private void RaiseUnderHiddenParentChanged()
        {
            foreach (var child in Children)
            {
                child.OnPropertyChanged(nameof(IsUnderHiddenParent));
                child.RaiseUnderHiddenParentChanged();
            }
        }
        public void Dispose()
        {
            if (Item is IPrefab prefab)
                prefab.PrefabService.Name.PropertyChanged -= OnNameChanged;

            if (hiddenSource != null)
                hiddenSource.PropertyChanged -= OnHiddenChanged;

            if (ChildManager != null)
            {
                ChildManager.ManagerData.Items.CollectionChanged -= OnItemsChanged;
                ChildManager.ManagerData.PropertyChanged -= OnManagerDataChanged;
            }

            foreach (var child in Children)
                child.Dispose();
        }
    }
}
