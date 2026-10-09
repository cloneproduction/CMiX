// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class EditableTextBox : UserControl
    {
        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<EditableTextBox, bool>(nameof(IsEditing));
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<EditableTextBox, bool>(nameof(IsSelected));
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<EditableTextBox, string>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private TopLevel? _parentTopLevel;

        static EditableTextBox()
        {
            IsEditingProperty.Changed.AddClassHandler<EditableTextBox>((textbox, e) =>
            {
                if (!e.GetNewValue<bool>()) return;

                textbox.OnSwitchToEditingMode();
                textbox.InputValue.Focus();
                textbox.InputValue.SelectAll();
            });
        }

        public EditableTextBox()
        {
            InitializeComponent();
            SetInputVisible(false);
            DoubleTapped += EditableTextBox_DoubleTapped;
            AddHandler(PointerPressedEvent, EditableTextBox_PointerPressed, RoutingStrategies.Tunnel);
        }

        private void SetInputVisible(bool editing)
        {
            InputValue.IsVisible = editing;
            TextDisplay.IsVisible = !editing;
        }

        private void EditableTextBox_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            var item = this.FindAncestorOfType<ListBoxItem>();
            if (item != null)
                item.IsSelected = true;
        }

        private void EditableTextBox_DoubleTapped(object? sender, TappedEventArgs e)
        {
            OnSwitchToEditingMode();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                OnSwitchToNormalMode();
                e.Handled = true;
                return;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            OnSwitchToNormalMode();
            base.OnLostFocus(e);
        }

        private void OnSwitchToEditingMode()
        {
            SetInputVisible(true);
            HookTopLevelEvents();
            InputValue.Text = Text;
            InputValue.Focus();
            InputValue.SelectAll();
        }

        private void OnSwitchToNormalMode()
        {
            Text = InputValue.Text ?? string.Empty;
            SetInputVisible(false);
            UnhookTopLevelEvents();
            IsEditing = false;
        }

        private void HookTopLevelEvents()
        {
            _parentTopLevel = TopLevel.GetTopLevel(this);
            if (_parentTopLevel != null)
            {
                _parentTopLevel.AddHandler(PointerWheelChangedEvent, ParentTopLevel_PointerWheelChanged, RoutingStrategies.Bubble, true);
                _parentTopLevel.AddHandler(PointerPressedEvent, ParentTopLevel_PointerPressed, RoutingStrategies.Tunnel);
                _parentTopLevel.SizeChanged += ParentTopLevel_SizeChanged;
            }
        }

        private void UnhookTopLevelEvents()
        {
            if (_parentTopLevel != null)
            {
                _parentTopLevel.RemoveHandler(PointerWheelChangedEvent, ParentTopLevel_PointerWheelChanged);
                _parentTopLevel.RemoveHandler(PointerPressedEvent, ParentTopLevel_PointerPressed);
                _parentTopLevel.SizeChanged -= ParentTopLevel_SizeChanged;
                _parentTopLevel = null;
            }
        }

        private void ParentTopLevel_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            OnSwitchToNormalMode();
        }

        private void ParentTopLevel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!InputValue.IsPointerOver)
                OnSwitchToNormalMode();
        }

        private void ParentTopLevel_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            OnSwitchToNormalMode();
        }
    }
}
