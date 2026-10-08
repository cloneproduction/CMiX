// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class CaptionedToggleButton : CaptionedUserControl
    {
        public CaptionedToggleButton()
        {
            InitializeComponent();
            labelBorder.PointerPressed += (s, e) =>
            {
                if (!e.GetCurrentPoint(labelBorder).Properties.IsLeftButtonPressed)
                    return;

                toggleButton.IsChecked = toggleButton.IsChecked != true;
                e.Handled = true;
            };
        }

        public static readonly StyledProperty<bool> IsCheckedProperty =
            AvaloniaProperty.Register<CaptionedToggleButton, bool>(nameof(IsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsChecked
        {
            get => GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly StyledProperty<ICommand> ResetCommandProperty =
            AvaloniaProperty.Register<CaptionedToggleButton, ICommand>(nameof(ResetCommand));
        public ICommand ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        private ContextMenu? _resetMenu;
        private bool _attached;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _attached = true;
            UpdateResetMenu();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (_attached && change.Property == ResetCommandProperty)
                UpdateResetMenu();
        }

        private void UpdateResetMenu() => DefaultResetMenu.Update(this, ResetCommand, ref _resetMenu);
    }
}
