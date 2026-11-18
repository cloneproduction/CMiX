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
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // A row of the project tree. Item is null for a group row.
    public sealed partial class TreeNode : ObservableObject, IDisposable
    {
        private readonly TreeNodeHost host;
        private readonly RowCounts counts;

        internal TreeNode(TreeNodeHost host, TreeNode? parent, PrefabManager? owner, IControl? item, string header, PrefabManager? childManager, TreeGroup? group = null)
        {
            this.host = host;
            counts = new RowCounts(this, parent?.counts);

            // A row made while the project loads starts collapsed.
            isExpanded = !host.IsLoading;
            Parent = parent;
            Owner = owner;
            Item = item;
            Group = group;
            this.header = group?.Header ?? header;
            IconKey = group?.IconKey ?? TreeIcons.OfItem(item);

            if (item is IPrefab prefab)
            {
                this.header = prefab.PrefabService.Name.Value;
                prefab.PrefabService.Name.PropertyChanged += OnNameChanged;
            }

            Category = item != null ? TreeCategories.Of(item) : group?.Category;
            kindLabel = item != null ? AddableTypes.LabelOf(item.GetType()) : null;

            hiddenSource = HiddenSourceOf(group, item);
            if (hiddenSource != null)
                hiddenSource.PropertyChanged += OnHiddenChanged;
            // A group of composables takes no level of its own, so the colors do not change.
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

            // The groups come after the items, so the index of an item in its list is its index in the children.
            if (item != null)
                AddGroups(item);

            counts.Count();
            counts.WatchAsset();
            Children.CollectionChanged += OnChildrenChanged;
        }

        private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e) => counts.RecountAll();

        internal void AddGroups(IControl item)
        {
            foreach (var group in TreeGroup.GroupsOf(item))
                Children.Add(new TreeNode(host, this, null, null, group.Header, group.Manager, group));
        }

        public TreeNode? Parent { get; }
        public string IconKey { get; }
        public IControl? Item { get; }
        public TreeGroup? Group { get; }

        public TreeCategory? Category { get; }

        public int ComposableDepth { get; }

        // A row is one step darker for each change of kind above it.
        public int KindDepth { get; }

        public object? InspectorContent => (object?)Item ?? Group;
        public PrefabManager? Owner { get; }
        public PrefabManager? ChildManager { get; }
        public ObservableCollection<TreeNode> Children { get; } = new();

        public IReadOnlyList<KindCount> KindCounts => counts.KindCounts;

        internal int KindCountsComputations => counts.KindCountsComputations;

        public bool HasKindCounts => counts.HasKindCounts;

        public const int MaxShownKindCounts = 3;

        public IReadOnlyList<KindCount> ShownKindCounts => counts.ShownKindCounts;

        public int MoreKindsCount => counts.MoreKindsCount;

        public bool HasMoreKinds => MoreKindsCount > 0;

        public string MoreKindsDescription => counts.MoreKindsDescription;

        public IControl? MaskOwner => Item is Composition or Layer ? Item : null;

        public bool ShowsKindCounts => (HasKindCounts || HasHiddenRows) && !IsExpanded;

        // The missing row always shows the sign, so the sign shows at every level down to the file.
        public bool ShowsMissingSign => HasMissingAssets && (IsMissingAsset || !IsExpanded);

        public bool ShowsBadges => ShowsKindCounts || ShowsMissingSign;

        public bool IsMissingAsset => counts.IsMissingAsset;

        public int MissingCount => counts.MissingCount;

        public bool HasMissingAssets => counts.HasMissingAssets;

        internal void RaiseKindCountsChanged()
        {
            OnPropertyChanged(nameof(KindCounts));
            OnPropertyChanged(nameof(HasKindCounts));
            OnPropertyChanged(nameof(ShownKindCounts));
            OnPropertyChanged(nameof(MoreKindsCount));
            OnPropertyChanged(nameof(HasMoreKinds));
            OnPropertyChanged(nameof(MoreKindsDescription));
            OnPropertyChanged(nameof(ShowsKindCounts));
            OnPropertyChanged(nameof(ShowsBadges));
        }

        internal void RaiseMissingCountChanged()
        {
            OnPropertyChanged(nameof(MissingCount));
            OnPropertyChanged(nameof(HasMissingAssets));
            OnPropertyChanged(nameof(ShowsMissingSign));
            OnPropertyChanged(nameof(ShowsBadges));
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

        // A row that the user added after the load. It fades in.
        public bool IsFresh { get; private set; }

        [ObservableProperty]
        private bool isCursor;

        [ObservableProperty]
        private bool isOnCursorBranch;

        internal static void MarkCursorBranch(TreeNode? cursor)
        {
            for (var parent = cursor?.Parent; parent != null; parent = parent.Parent)
                parent.IsOnCursorBranch = true;
        }

        internal static void ClearCursorBranch(TreeNode? cursor)
        {
            for (var parent = cursor?.Parent; parent != null; parent = parent.Parent)
                parent.IsOnCursorBranch = false;
        }

        // A candidate takes the node inside. A neighbor takes it before or after.
        // A blocked row takes the kind but refuses the node by a rule.
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

        public string? DropBlockReason { get => dropBlockReason; private set => SetProperty(ref dropBlockReason, value); }
        public bool IsDropBlocked => DropBlockReason != null;
        public bool IsDropHint => IsDragActive && HasDropTargetBelow && !IsDropCandidate && !IsDropNeighbor && !IsDropBlocked && !IsExpanded;
        // A blocked row is dimmed too. Its tooltip gives the reason.
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

        // The type name as the Add panel shows it, so a renamed item is found by its type.
        private readonly string? kindLabel;

        private bool isSearchMatch;
        private bool isSearchDimmed;

        public bool IsSearchMatch { get => isSearchMatch; private set => SetProperty(ref isSearchMatch, value); }
        public bool IsSearchDimmed { get => isSearchDimmed; private set => SetProperty(ref isSearchDimmed, value); }

        internal bool MatchesSearch(string text) =>
            Header.Contains(text, StringComparison.OrdinalIgnoreCase)
            || (kindLabel != null && kindLabel.Contains(text, StringComparison.OrdinalIgnoreCase));

        internal void ShowSearchLook(SearchLook look)
        {
            IsSearchMatch = look.IsMatch;
            IsSearchDimmed = !look.IsMatch;
        }

        internal void ClearSearchLook()
        {
            IsSearchMatch = false;
            IsSearchDimmed = false;
        }

        partial void OnHeaderChanged(string value) => host.RefreshSearch();

        partial void OnIsExpandedChanged(bool value) => host.RowExpandedChanged(this);

        public bool IsOrIsUnder(TreeNode node)
        {
            for (var current = this; current != null; current = current.Parent)
                if (ReferenceEquals(current, node))
                    return true;
            return false;
        }

        private TreeNode CreateItemNode(IControl item) =>
            new(host, this, ChildManager, item, string.Empty, null);

        private void OnNameChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value" && Item is IPrefab prefab)
                Header = prefab.PrefabService.Name.Value;
        }

        private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when host.IsLoading:
                    InsertChildren(e.NewStartingIndex, e.NewItems!.Cast<IControl>());
                    break;
                case NotifyCollectionChangedAction.Add:
                    InsertFreshChildren(e.NewStartingIndex, e.NewItems!.Cast<IControl>());

                    // A new item shows at once. A loaded item opens nothing.
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

            host.ListChanged();
        }

        private void InsertChildren(int index, IEnumerable<IControl> items)
        {
            foreach (var item in items)
                Children.Insert(index++, CreateItemNode(item));
        }

        private void InsertFreshChildren(int index, IEnumerable<IControl> items)
        {
            foreach (var item in items)
            {
                var node = CreateItemNode(item);
                node.IsFresh = true;
                Children.Insert(index++, node);
            }
        }

        private void RemoveChildren(int index, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var node = Children[index];
                host.NodeRemoved(node);
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
                host.SelectFromModel(node);
        }

        // The engine draws nothing under a hidden row, but the rows under it keep their own state.
        private readonly INotifyPropertyChanged? hiddenSource;

        private static INotifyPropertyChanged? HiddenSourceOf(TreeGroup? group, IControl? item) =>
            group != null ? group.Manager.Hidden : (item as IPrefab)?.PrefabService.Visibility;

        public bool IsHidden => hiddenSource switch
        {
            GenericValue<bool> value => value.Value,
            _ => false
        };

        public GenericValue<bool>? HiddenValue => hiddenSource as GenericValue<bool>;

        // The toggles look greyed but work: the row keeps its own visibility.
        public bool IsUnderHiddenParent => TreeWalk.ParentsOf(this).Any(parent => parent.IsHidden);

        private void OnHiddenChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
                return;

            OnPropertyChanged(nameof(IsHidden));
            RaiseUnderHiddenParentChanged();
            counts.RecountHiddenAbove();
        }

        public int HiddenCount => counts.HiddenCount;

        public bool HasHiddenRows => counts.HasHiddenRows;

        internal void RaiseHiddenCountChanged() => OnPropertyChanged(nameof(HiddenCount));

        internal void RaiseHasHiddenRowsChanged()
        {
            OnPropertyChanged(nameof(HasHiddenRows));
            OnPropertyChanged(nameof(ShowsKindCounts));
            OnPropertyChanged(nameof(ShowsBadges));
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
