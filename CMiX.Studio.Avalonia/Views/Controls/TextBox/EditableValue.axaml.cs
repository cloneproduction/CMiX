// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class EditableValue : UserControl
    {
        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<EditableValue, bool>(nameof(IsEditing));
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<EditableValue, string>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private TopLevel _parentTopLevel;

        static EditableValue()
        {
            IsEditingProperty.Changed.AddClassHandler<EditableValue>((textbox, e) =>
            {
                if (e.GetNewValue<bool>())
                    textbox.OnSwitchToEditingMode();
            });
        }

        public EditableValue()
        {
            InitializeComponent();
            SetInputVisible(false);
            InputValue.AddHandler(TextInputEvent, TextInput_OnTextInput, RoutingStrategies.Tunnel);
        }

        private void SetInputVisible(bool editing)
        {
            InputValue.IsVisible = editing;
            TextDisplay.IsVisible = !editing;
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
            e.Handled = true;
            OnSwitchToNormalMode();
        }

        private void OnSwitchToEditingMode()
        {
            HookTopLevelEvents();
            SetInputVisible(true);
            InputValue.Text = Text;
            InputValue.Focus();
            InputValue.SelectAll();
        }

        private void OnSwitchToNormalMode()
        {
            IsEditing = false;
            Text = InputValue.Text;
            SetInputVisible(false);
            UnhookTopLevelEvents();
        }

        private void HookTopLevelEvents()
        {
            _parentTopLevel = TopLevel.GetTopLevel(this);
            _parentTopLevel?.AddHandler(PointerPressedEvent, TopLevel_PointerPressed, RoutingStrategies.Tunnel);
        }

        private void UnhookTopLevelEvents()
        {
            if (_parentTopLevel != null)
            {
                _parentTopLevel.RemoveHandler(PointerPressedEvent, TopLevel_PointerPressed);
                _parentTopLevel = null;
            }
        }

        private void TopLevel_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!InputValue.IsPointerOver)
                OnSwitchToNormalMode();
        }

        // Regex that matches disallowed text.
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextInput_OnTextInput(object sender, TextInputEventArgs e)
        {
            e.Handled = e.Text != null && !IsTextAllowed(e.Text);
        }
    }
}
