// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;

namespace CMiX.UI.Controls.String
{
    public partial class TextBoxEditable : UserControl
    {
        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<TextBoxEditable, bool>(nameof(IsEditing));
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<TextBoxEditable, bool>(nameof(IsSelected));
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<TextBoxEditable, string>(nameof(Text), defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<object?> SuffixProperty =
            AvaloniaProperty.Register<TextBoxEditable, object?>(nameof(Suffix));
        public object? Suffix
        {
            get => GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        private TopLevel? _parentTopLevel;
        private readonly ContextMenuGuard _contextMenuGuard;

        static TextBoxEditable()
        {
            IsEditingProperty.Changed.AddClassHandler<TextBoxEditable>((textbox, e) =>
            {
                if (!e.GetNewValue<bool>()) return;

                textbox.OnSwitchToEditingMode();
                textbox.inputValue.Focus();
                textbox.inputValue.SelectAll();
            });
        }

        public TextBoxEditable()
        {
            InitializeComponent();
            SetInputVisible(false);
            // The menu of a row must not open in the edit mode. A double click starts the edit mode without IsEditing, so the input tells.
            _contextMenuGuard = ContextMenuGuard.Install(this, () => inputValue.IsVisible);
            DoubleTapped += TextBoxEditable_DoubleTapped;
            AddHandler(PointerPressedEvent, TextBoxEditable_PointerPressed, RoutingStrategies.Tunnel);
        }

        private void SetInputVisible(bool editing)
        {
            inputValue.IsVisible = editing;
            textDisplay.IsVisible = !editing;
            displayRow.IsVisible = !editing;
        }

        private void TextBoxEditable_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            var item = this.FindAncestorOfType<ListBoxItem>();
            if (item != null)
                item.IsSelected = true;
        }

        // Only a double tap on the text starts the edit mode. A double tap on the rest of the row is for the row:
        // a row of a tree expands, a row of a list does nothing. A double tap on the text, or in the input while it is edited
        // (to select a word), is taken here, so the row does not see it.
        private void TextBoxEditable_DoubleTapped(object? sender, TappedEventArgs e)
        {
            if (ReferenceEquals(e.Source, textDisplay))
            {
                OnSwitchToEditingMode();
                e.Handled = true;
            }
            else if (e.Source is Visual source && (ReferenceEquals(source, inputValue) || inputValue.IsVisualAncestorOf(source)))
            {
                e.Handled = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CancelEdit();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                OnSwitchToNormalMode();
                e.Handled = true;
                return;
            }
        }

        // The edit ends and the old text stays. The input has no binding, so Text still holds the old text.
        private void CancelEdit()
        {
            SetInputVisible(false);
            UnhookTopLevelEvents();
            IsEditing = false;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            CancelEdit();
            base.OnDetachedFromVisualTree(e);
        }

        // True while a handler of the top level is hooked. For the tests.
        internal bool HasTopLevelHandlers => _parentTopLevel != null;

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            OnSwitchToNormalMode();
            base.OnLostFocus(e);
        }

        private void OnSwitchToEditingMode()
        {
            SetInputVisible(true);
            HookTopLevelEvents();
            inputValue.Text = Text;
            inputValue.Focus();
            inputValue.SelectAll();
        }

        // The edit ends with the text of the input. An empty text is not a name: the old text stays.
        // Only the input that is shown holds a text to commit. A hidden input holds the text of the last edit.
        private void OnSwitchToNormalMode()
        {
            if (inputValue.IsVisible && !string.IsNullOrWhiteSpace(inputValue.Text))
                Text = inputValue.Text;

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

        // A right click cancels the edit, also over the input, and does nothing else: the old name stays, and no menu opens
        // when the button is released. A left click outside the input ends the edit with the typed name, and the click goes on.
        private void ParentTopLevel_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                CancelEdit();
                OutsideClick.Take(e);
                return;
            }

            if (!inputValue.IsPointerOver)
                OnSwitchToNormalMode();
        }

        private void ParentTopLevel_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            OnSwitchToNormalMode();
        }
    }
}
