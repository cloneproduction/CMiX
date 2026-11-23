// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class RowListTree : UserControl
    {
        public const string CardClass = ":card";
        public const string SelectedClass = ":selected";
        public const string InSelectionClass = ":inselection";
        public const string InHoveredCardClass = ":inhoveredcard";
        public const string HiddenClass = ":hiddenrow";
        public const string DropTargetClass = ":droptarget";
        public const string DimmedClass = ":dimmed";
        public const string DraggedClass = ":dragged";

        public static readonly StyledProperty<ListTreePlace?> PlaceProperty =
            AvaloniaProperty.Register<RowListTree, ListTreePlace?>(nameof(Place));

        public static readonly StyledProperty<string?> IconKeyProperty =
            AvaloniaProperty.Register<RowListTree, string?>(nameof(IconKey));

        // TreeRowBrush reads IsSelected and ItemCount, the same as on the TreeViewItem of the old tree.
        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<RowListTree, bool>(nameof(IsSelected));

        public static readonly StyledProperty<int> ItemCountProperty =
            AvaloniaProperty.Register<RowListTree, int>(nameof(ItemCount));

        public static readonly StyledProperty<Thickness> IndentMarginProperty =
            AvaloniaProperty.Register<RowListTree, Thickness>(nameof(IndentMargin));

        public static readonly StyledProperty<IBrush?> IconForegroundProperty =
            AvaloniaProperty.Register<RowListTree, IBrush?>(nameof(IconForeground));

        public RowListTree()
        {
            InitializeComponent();
        }

        public ListTreePlace? Place
        {
            get => GetValue(PlaceProperty);
            set => SetValue(PlaceProperty, value);
        }

        public string? IconKey
        {
            get => GetValue(IconKeyProperty);
            set => SetValue(IconKeyProperty, value);
        }

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public int ItemCount
        {
            get => GetValue(ItemCountProperty);
            set => SetValue(ItemCountProperty, value);
        }

        public Thickness IndentMargin
        {
            get => GetValue(IndentMarginProperty);
            private set => SetValue(IndentMarginProperty, value);
        }

        public IBrush? IconForeground
        {
            get => GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        internal Control IconHit => iconHit;

        internal TogglesRow Toggles => toggles;

        internal Border Band => band;

        // The name box of an item, the text of a group.
        internal Control NameText =>nameBox.IsVisible ? nameBox : groupName;

        internal void StartRename() => nameBox.IsEditing = true;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PlaceProperty)
                ShowPlace(Place);
            else if (change.Property == IsSelectedProperty)
                PseudoClasses.Set(SelectedClass, IsSelected);
        }

        private void ShowPlace(ListTreePlace? place)
        {
            IndentMargin = new Thickness(place?.Indent ?? 0, 0, 0, 0);
            PseudoClasses.Set(CardClass, place?.CanToggle == true);
        }

        public void ShowMarks(ListTreeMarks marks)
        {
            var node = DataContext as TreeNode;
            IsSelected = marks.IsSelected(node);
            PseudoClasses.Set(InSelectionClass, marks.IsInSelection(node));
            PseudoClasses.Set(InHoveredCardClass, marks.IsInHoveredCard(node));
            PseudoClasses.Set(DropTargetClass, marks.IsDropTarget(node));
            PseudoClasses.Set(HiddenClass, marks.IsHidden(node));
        }

        public void ShowLight(ListTreeRowLight light)
        {
            PseudoClasses.Set(DimmedClass, light == ListTreeRowLight.Dimmed);
            PseudoClasses.Set(DraggedClass, light == ListTreeRowLight.Dragged);
        }

        internal bool IsCardIcon(object? source) =>
            Place is { CanToggle: true } && source is Visual visual && (ReferenceEquals(visual, iconHit) || iconHit.IsVisualAncestorOf(visual));

        private void OnIconTapped(object? sender, TappedEventArgs e) => ToggleCard();

        internal void ToggleCard()
        {
            if (Place is not { CanToggle: true } || DataContext is not TreeNode node)
                return;

            node.ToggleExpand();
        }
    }
}
