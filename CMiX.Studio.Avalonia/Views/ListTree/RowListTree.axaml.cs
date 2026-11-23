// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class RowListTree : UserControl
    {
        public const string CardClass = ":card";

        public static readonly StyledProperty<ListTreePlace?> PlaceProperty =
            AvaloniaProperty.Register<RowListTree, ListTreePlace?>(nameof(Place));

        public static readonly StyledProperty<string?> IconKeyProperty =
            AvaloniaProperty.Register<RowListTree, string?>(nameof(IconKey));

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

        internal Control IconHit => iconHit;

        internal TogglesRow Toggles => toggles;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PlaceProperty)
                ShowPlace(Place);
        }

        private void ShowPlace(ListTreePlace? place)
        {
            Padding = new Thickness(place?.Indent ?? 0, 0, 0, 0);
            PseudoClasses.Set(CardClass, place?.CanToggle == true);
        }

        private void OnIconTapped(object? sender, TappedEventArgs e) => ToggleCard();

        internal void ToggleCard()
        {
            if (Place is not { CanToggle: true } || DataContext is not TreeNode node)
                return;

            node.IsExpanded = !node.IsExpanded;
        }
    }
}
