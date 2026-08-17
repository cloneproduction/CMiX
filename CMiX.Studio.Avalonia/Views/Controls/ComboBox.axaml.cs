// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaComboBox = Avalonia.Controls.ComboBox;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ComboBox : UserControl
    {
        private AvaloniaComboBox? _innerComboBox;

        public ComboBox()
        {
            InitializeComponent();
            _innerComboBox = this.FindControl<AvaloniaComboBox>("innerComboBox");

            _innerComboBox?.AddHandler(PointerWheelChangedEvent, OnInnerPointerWheelChanged, RoutingStrategies.Tunnel);
            _innerComboBox?.AddHandler(PointerPressedEvent, OnInnerPointerPressed, RoutingStrategies.Tunnel);

            if (_innerComboBox != null)
                _innerComboBox.PropertyChanged += OnInnerComboBoxPropertyChanged;
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ComboBox, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<ComboBox, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<ComboBox, object?>(nameof(SelectedItem), null, defaultBindingMode: BindingMode.TwoWay);
        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private bool IsComboBoxFocused()
        {
            var focused = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as Visual;
            if (focused == null || _innerComboBox == null)
                return false;

            return focused == _innerComboBox || focused.GetVisualAncestors().Contains(_innerComboBox);
        }

        private void OnInnerPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            bool shouldCycleSelection =
                IsComboBoxFocused() &&
                _innerComboBox != null &&
                !_innerComboBox.IsDropDownOpen &&
                ItemsSource != null;

            if (!shouldCycleSelection)
                return; // leave e.Handled false: bubbles up normally (dropdown scroll, or an ancestor ScrollViewer)

            List<object> items = ItemsSource!.Cast<object>().ToList();
            if (items.Count == 0)
                return;

            int currentIndex = SelectedItem != null ? items.IndexOf(SelectedItem) : -1;
            int newIndex = currentIndex - (int)e.Delta.Y;

            if (newIndex < 0)
                newIndex = 0;
            else if (newIndex >= items.Count)
                newIndex = items.Count - 1;

            if (newIndex != currentIndex)
                SelectedItem = items[newIndex];

            e.Handled = true;
        }

        private void OnInnerPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _innerComboBox?.Focus();        }

        private void OnInnerComboBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            // Closing the dropdown (without necessarily picking a new item) does not return
            // focus to the ComboBox on its own, unlike WPF. Re-focus explicitly so wheel-to-cycle
            // keeps working right after the dropdown is dismissed by clicking the control again.
            if (e.Property == AvaloniaComboBox.IsDropDownOpenProperty && e.NewValue is false)
                _innerComboBox?.Focus();
        }
    }
}
