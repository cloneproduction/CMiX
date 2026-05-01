// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.Controls
{
    public class NumericInput : Control
    {
        static NumericInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericInput), new FrameworkPropertyMetadata(typeof(NumericInput)));
        }

        private TextBox _textBox;
        private double _valueBeforeEdit;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            if (_textBox != null)
            {
                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.MouseLeave += TextBox_MouseLeave;
                _textBox.MouseEnter += TextBox_MouseEnter;
                _textBox.PreviewMouseDoubleClick += TextBox_PreviewMouseDoubleClick;
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericInput),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(NumericInput),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsEditingChanged));
        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericInput),
                new FrameworkPropertyMetadata(double.MinValue));
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericInput),
                new FrameworkPropertyMetadata(double.MaxValue));
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty IsIntegerProperty =
            DependencyProperty.Register("IsInteger", typeof(bool), typeof(NumericInput),
                new FrameworkPropertyMetadata(false));
        public bool IsInteger
        {
            get => (bool)GetValue(IsIntegerProperty);
            set => SetValue(IsIntegerProperty, value);
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericInput)d;
            if ((bool)e.NewValue)
                control.EnterEditMode();
            else
                control.ExitEditMode();
        }

        private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _textBox.SelectAll();
            e.Handled = true;
        }

        private void EnterEditMode()
        {
            if (_textBox == null) return;
            _valueBeforeEdit = Value;
            _textBox.Text = IsInteger ? ((int)Value).ToString() : Value.ToString("N3");
            this.Visibility = Visibility.Visible;
            AddParentWindowHandlers();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _textBox.Focus();
                _textBox.SelectAll();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void ExitEditMode()
        {
            this.Visibility = Visibility.Hidden;
            RemoveParentWindowHandlers();
            Keyboard.ClearFocus();
            var w = Window.GetWindow(this);
            w?.Focus();
        }

        private void Commit()
        {
            if (_textBox == null) return;
            if (double.TryParse(_textBox.Text, out double result))
                Value = Math.Clamp(result, Minimum, Maximum);
            else
                _textBox.Text = Value.ToString();
            IsEditing = false;
        }

        private void Cancel()
        {
            if (_textBox == null) return;
            Value = _valueBeforeEdit;
            _textBox.Text = _valueBeforeEdit.ToString();
            IsEditing = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Commit();
            else if (e.Key == Key.Escape)
                Cancel();
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsEditing)
                AddParentWindowHandlers();
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void AddParentWindowHandlers()
        {
            var w = Window.GetWindow(this);
            if (w != null)
                Mouse.AddPreviewMouseDownHandler(w, ParentWindow_OnMouseDown);
        }

        private void RemoveParentWindowHandlers()
        {
            var w = Window.GetWindow(this);
            if (w != null)
                Mouse.RemovePreviewMouseDownHandler(w, ParentWindow_OnMouseDown);
        }

        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveParentWindowHandlers();
            if (!IsEditing) return;

            if (e.ChangedButton == MouseButton.Right)
                Cancel();
            else
                Commit();

            e.Handled = true;
        }
    }
}
