// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    [TemplatePart("PART_TextBox", typeof(TextBox))]
    public class NumericInput : TemplatedControl
    {
        private TextBox _textBox;
        private double _valueBeforeEdit;

        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<NumericInput, double>(nameof(Value), 0.0, defaultBindingMode: BindingMode.TwoWay);
        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly StyledProperty<bool> IsEditingProperty =
            AvaloniaProperty.Register<NumericInput, bool>(nameof(IsEditing), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsEditing
        {
            get => GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<NumericInput, double>(nameof(Minimum), double.MinValue);
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<NumericInput, double>(nameof(Maximum), double.MaxValue);
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly StyledProperty<bool> IsIntegerProperty =
            AvaloniaProperty.Register<NumericInput, bool>(nameof(IsInteger));
        public bool IsInteger
        {
            get => GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }

        static NumericInput()
        {
            IsEditingProperty.Changed.AddClassHandler<NumericInput>(OnIsEditingChanged);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            if (_textBox != null)
            {
                _textBox.KeyDown -= TextBox_KeyDown;
                _textBox.PointerExited -= TextBox_PointerExited;
                _textBox.PointerEntered -= TextBox_PointerEntered;
                _textBox.DoubleTapped -= TextBox_DoubleTapped;
            }

            _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
            if (_textBox != null)
            {
                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.PointerExited += TextBox_PointerExited;
                _textBox.PointerEntered += TextBox_PointerEntered;
                _textBox.DoubleTapped += TextBox_DoubleTapped;
            }
        }

        private static void OnIsEditingChanged(NumericInput control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.GetNewValue<bool>())
                control.EnterEditMode();
            else
                control.ExitEditMode();
        }

        private void TextBox_DoubleTapped(object sender, TappedEventArgs e)
        {
            _textBox.SelectAll();
            e.Handled = true;
        }

        private void EnterEditMode()
        {
            // The theme keeps the control hidden through IsVisible until editing starts,
            // and Avalonia never measures an invisible control, so the template is not
            // applied yet on the first edit. Become visible first and force the template
            // so PART_TextBox exists. The control overlaps the value display in the same
            // grid cell, so toggling IsVisible causes no layout shift.
            IsVisible = true;
            ApplyTemplate();
            if (_textBox == null) return;

            _valueBeforeEdit = Value;
            _textBox.Text = IsInteger ? ((int)Value).ToString() : Value.ToString("N3");
            AddParentWindowHandlers();
            Dispatcher.UIThread.Post(() =>
            {
                _textBox.Focus();
                _textBox.SelectAll();
            }, DispatcherPriority.Loaded);
        }

        private void ExitEditMode()
        {
            IsVisible = false;
            RemoveParentWindowHandlers();
            var topLevel = TopLevel.GetTopLevel(this);
            topLevel?.Focus();
        }

        private void Commit()
        {
            if (_textBox != null)
            {
                if (double.TryParse(_textBox.Text, out double result))
                    Value = Math.Clamp(result, Minimum, Maximum);
                else
                    _textBox.Text = Value.ToString();
            }
            IsEditing = false;
        }

        private void Cancel()
        {
            if (_textBox != null)
            {
                Value = _valueBeforeEdit;
                _textBox.Text = _valueBeforeEdit.ToString();
            }
            IsEditing = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Commit();
            else if (e.Key == Key.Escape)
                Cancel();
        }

        private void TextBox_PointerExited(object sender, PointerEventArgs e)
        {
            if (IsEditing)
                AddParentWindowHandlers();
        }

        private void TextBox_PointerEntered(object sender, PointerEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void AddParentWindowHandlers()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                RemoveParentWindowHandlers();
                topLevel.AddHandler(InputElement.PointerPressedEvent, ParentWindow_OnPointerPressed, RoutingStrategies.Tunnel);
            }
        }

        private void RemoveParentWindowHandlers()
        {
            var topLevel = TopLevel.GetTopLevel(this);
            topLevel?.RemoveHandler(InputElement.PointerPressedEvent, ParentWindow_OnPointerPressed);
        }

        private void ParentWindow_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            RemoveParentWindowHandlers();
            if (!IsEditing) return;

            if (e.GetCurrentPoint(sender as Visual).Properties.IsRightButtonPressed)
                Cancel();
            else
                Commit();

            e.Handled = true;
        }
    }
}
