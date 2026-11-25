// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.UI.Search;
using CMiX.UI.Themes;

namespace CMiX.UI.Controls.Collection
{
    // A list of groups under a search field. A header shows its row count. A group over CollapseFrom rows starts closed.
    // Right and Left open and close a header, Escape raises Cancelled. PickerKeys holds Down, Up, Enter and Escape.
    public partial class ListGroupedSearch : UserControl, IPickerRows
    {
        private sealed record RowRef(PickerGroup Group, PickerItem? Item, Border Visual);

        private readonly List<RowRef> rows = new();
        private readonly HashSet<PickerGroup> flipped = new();
        private readonly PickerKeys keys;
        private int highlighted = -1;

        static ListGroupedSearch()
        {
            GroupsProperty.Changed.AddClassHandler<ListGroupedSearch>((list, _) => list.RebuildIfAttached());
            SearchTextProperty.Changed.AddClassHandler<ListGroupedSearch>((list, _) => list.RebuildIfAttached());
            CollapseFromProperty.Changed.AddClassHandler<ListGroupedSearch>((list, _) => list.RebuildIfAttached());
            CollapsibleProperty.Changed.AddClassHandler<ListGroupedSearch>((list, _) => list.RebuildIfAttached());
        }

        public ListGroupedSearch()
        {
            keys = new PickerKeys(this);
            InitializeComponent();
            PART_Search.KeyDown += OnSearchKeyDown;
        }

        public static readonly StyledProperty<IEnumerable<PickerGroup>?> GroupsProperty =
            AvaloniaProperty.Register<ListGroupedSearch, IEnumerable<PickerGroup>?>(nameof(Groups));

        public IEnumerable<PickerGroup>? Groups
        {
            get => GetValue(GroupsProperty);
            set => SetValue(GroupsProperty, value);
        }

        public static readonly StyledProperty<string> SearchTextProperty =
            AvaloniaProperty.Register<ListGroupedSearch, string>(nameof(SearchText), string.Empty);

        public string SearchText
        {
            get => GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        // A group with more rows than this starts closed.
        public static readonly StyledProperty<int> CollapseFromProperty =
            AvaloniaProperty.Register<ListGroupedSearch, int>(nameof(CollapseFrom), 8);

        public int CollapseFrom
        {
            get => GetValue(CollapseFromProperty);
            set => SetValue(CollapseFromProperty, value);
        }

        // True when a click on a header opens and closes its group. Otherwise a header only names its category.
        public static readonly StyledProperty<bool> CollapsibleProperty =
            AvaloniaProperty.Register<ListGroupedSearch, bool>(nameof(Collapsible), true);

        public bool Collapsible
        {
            get => GetValue(CollapsibleProperty);
            set => SetValue(CollapsibleProperty, value);
        }

        // The number of rows that show before the list scrolls. A header counts as a row.
        public static readonly StyledProperty<int> VisibleRowsProperty =
            AvaloniaProperty.Register<ListGroupedSearch, int>(nameof(VisibleRows), 10);

        public int VisibleRows
        {
            get => GetValue(VisibleRowsProperty);
            set => SetValue(VisibleRowsProperty, value);
        }

        // The color of the icon of a row, from the Kind of the row. Without it, or when it gives null, the icon has the color of the text.
        public Func<object?, IBrush?>? KindBrush { get; set; }

        public event EventHandler<PickerPickedEventArgs>? Picked;

        // The user asked to leave the list: Escape with an empty field.
        public event EventHandler? Cancelled;

        // The text of the visible rows, in order: a header gives its text, and a row gives its text. A test reads them.
        public IReadOnlyList<string> VisibleTexts => rows.Select(row => row.Item?.Text ?? row.Group.Header).ToList();

        // The index of the highlighted row in the visible rows, or -1.
        public int HighlightedIndex => highlighted;

        public IReadOnlyList<Border> RowVisuals => rows.Select(row => row.Visual).ToList();

        // The state of a group: open or closed, as the user sees it.
        public bool IsOpen(PickerGroup group) => !Collapsible || Searching || StartsOpen(group) != flipped.Contains(group);

        // A group starts open when it is small and has a row that can be picked.
        private bool StartsOpen(PickerGroup group) => group.Items.Count <= CollapseFrom && group.HasEnabledItems;

        private bool Searching => !string.IsNullOrEmpty(SearchText);

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            // Each time the list appears it starts without a filter and with the first state of its groups.
            SearchText = string.Empty;
            flipped.Clear();
            Rebuild();
            PART_Search.FocusWhenShown();
        }

        private void RebuildIfAttached()
        {
            if (this.IsAttachedToVisualTree())
                Rebuild();
        }

        private void Rebuild()
        {
            RebuildRows();
            keys.Reset(FirstStart());
        }

        private void RebuildKeepingHeader(PickerGroup group)
        {
            RebuildRows();
            var start = rows.FindIndex(row => row.Item == null && row.Group == group);
            keys.Reset(start < 0 ? FirstStart() : start);
        }

        private void RebuildRows()
        {
            PART_Rows.Children.Clear();
            rows.Clear();

            var shown = 0;
            foreach (var group in Groups ?? Array.Empty<PickerGroup>())
            {
                var matches = Searching
                    ? group.Items.Where(item => SearchRules.Matches(SearchText, item.Text)).ToList()
                    : group.Items.ToList();
                if (Searching && matches.Count == 0)
                    continue;

                shown++;
                Add(new RowRef(group, null, HeaderRow(group)));
                if (IsOpen(group))
                {
                    foreach (var item in matches)
                        Add(new RowRef(group, item, ItemRow(item)));
                }
            }

            PART_NoMatch.IsVisible = Searching && shown == 0;
        }

        // The first row that can be picked, else the first header.
        private int FirstStart()
        {
            var start = rows.FindIndex(row => row.Item is { IsEnabled: true });
            return start < 0 && rows.Count > 0 ? 0 : start;
        }

        private void Add(RowRef row)
        {
            var index = rows.Count;
            rows.Add(row);
            PART_Rows.Children.Add(row.Visual);
            row.Visual.PointerEntered += (_, _) =>
            {
                if (IsUsable(index))
                    Highlight(index);
            };
            row.Visual.PointerReleased += (_, e) =>
            {
                if (e.InitialPressMouseButton == MouseButton.Left && IsUsable(index))
                {
                    Activate(index);
                    e.Handled = true;
                }
            };
        }

        private Border HeaderRow(PickerGroup group)
        {
            var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*") };

            // A list that is not collapsible shows the name of the category only.
            if (Collapsible)
                grid.Children.Add(new AppIcon { IconKey = IsOpen(group) ? "ChevronDown" : "ChevronRight", Size = IconSize, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });

            var text = new TextBlock { Text = group.Header, Margin = new Thickness(Collapsible ? 6 : 0, 0, 0, 0), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
            Grid.SetColumn(text, 1);
            grid.Children.Add(text);

            var row = new Border { Padding = new Thickness(Spacing, 0), Child = grid };
            row.Classes.Add("row");
            row.Classes.Add("header");
            if (!group.HasEnabledItems)
                row.Classes.Add("dimmed");

            return row;
        }

        // A row shows a small square in the color of its kind, when the list has a color for the kind.
        private Border ItemRow(PickerItem item)
        {
            var panel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = Spacing };
            if (KindBrush?.Invoke(item.Kind) is { } brush)
            {
                var size = IconSize * 0.8;
                panel.Children.Add(new Border
                {
                    Width = size, Height = size, Background = brush,
                    CornerRadius = ThemeResource.Get(this, "CornerRadiusAll", new CornerRadius(2)), VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                });
            }

            panel.Children.Add(new TextBlock { Text = item.Text, VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center });

            var row = new Border { Padding = new Thickness(Collapsible ? 24 : 12, 0, Spacing, 0), Child = panel };
            row.Classes.Add("row");
            if (!item.IsEnabled)
                row.Classes.Add("disabled");

            return row;
        }

        // A header and a row that is enabled can be highlighted, clicked and picked. A row that is not enabled cannot.
        private bool IsUsable(int index) =>
            index >= 0 && index < rows.Count && (rows[index].Item == null ? Collapsible : rows[index].Item!.IsEnabled);

        private double Spacing => ThemeResource.Get(this, "SpacingMD", 8.0);

        private double IconSize => ThemeResource.Get(this, "IconSizeXS", 16.0);

        private void ScrollToHighlight()
        {
            if (highlighted >= 0)
                rows[highlighted].Visual.BringIntoView();
        }

        private void Highlight(int index)
        {
            if (highlighted >= 0 && highlighted < rows.Count)
                rows[highlighted].Visual.Classes.Set("highlighted", false);

            highlighted = index;
            if (highlighted < 0)
                return;

            rows[highlighted].Visual.Classes.Set("highlighted", true);
        }

        // A header opens or closes its group. A row is picked.
        private void Activate(int index)
        {
            if (index < 0 || index >= rows.Count)
                return;

            var row = rows[index];
            if (row.Item is { IsEnabled: false })
                return;

            if (row.Item == null)
            {
                Toggle(row.Group);
                return;
            }

            Picked?.Invoke(this, new PickerPickedEventArgs(row.Group, row.Item));
        }

        int IPickerRows.Count => rows.Count;

        int IPickerRows.Highlighted => highlighted;

        bool IPickerRows.Searching => Searching;

        bool IPickerRows.IsUsable(int index) => IsUsable(index);

        void IPickerRows.Highlight(int index)
        {
            Highlight(index);
            ScrollToHighlight();
        }

        void IPickerRows.ClearSearch() => SearchText = string.Empty;

        // Enter is used even on a row that does nothing.
        bool IPickerRows.TryActivate(int index)
        {
            Activate(index);
            return true;
        }

        private void Toggle(PickerGroup group)
        {
            // While the list searches, every group is open and a click changes nothing.
            if (Searching)
                return;

            if (!flipped.Remove(group))
                flipped.Add(group);

            RebuildKeepingHeader(group);
        }

        private void Open(PickerGroup group)
        {
            if (!Searching && !IsOpen(group))
                Toggle(group);
        }

        private void Close(PickerGroup group)
        {
            if (!Searching && IsOpen(group))
                Toggle(group);
        }

        private void OnSearchKeyDown(object? sender, KeyEventArgs e)
        {
            var onHeader = highlighted >= 0 && highlighted < rows.Count && rows[highlighted].Item == null;
            switch (e.Key)
            {
                case Key.Right when onHeader && !Searching:
                    Open(rows[highlighted].Group);
                    e.Handled = true;
                    break;

                case Key.Left when onHeader && !Searching:
                    Close(rows[highlighted].Group);
                    e.Handled = true;
                    break;

                // Left with no text leaves the list as Escape does: the panel is a leaf of a tree, and Left goes back to the row.
                case Key.Left or Key.Escape when !Searching:
                    Cancelled?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;

                default:
                    keys.OnKeyDown(sender, e);
                    break;
            }
        }
    }
}
