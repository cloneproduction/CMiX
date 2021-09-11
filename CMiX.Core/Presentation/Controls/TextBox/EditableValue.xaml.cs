// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Core.Presentation.Controls
{
    public partial class EditableValue : UserControl
    {
        public EditableValue()
        {
            InitializeComponent();
            InputValue.Visibility = Visibility.Hidden;
            TextDisplay.Visibility = Visibility.Visible;
        }

        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableValue), new UIPropertyMetadata(false, new PropertyChangedCallback(IsEditing_PropertyChanged)));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        private static void IsEditing_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as EditableValue;
            textbox.OnSwitchToEditingMode();
        }

        [Bindable(true)]
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(EditableValue), new UIPropertyMetadata(new PropertyChangedCallback(TextProperty_PropertyChanged)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static void TextProperty_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EditableValue textbox = obj as EditableValue;
            var newtext = (string)e.NewValue;
            textbox.TextDisplay.Text = newtext;
            textbox.InputValue.Text = newtext;
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

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
            OnSwitchToNormalMode();
        }

        public void OnMouseDownOutsideElement(object sender, MouseButtonEventArgs e)
        { 
            OnSwitchToNormalMode();
            e.Handled = true;
        }



        private void OnSwitchToEditingMode()
        {
            this.CaptureMouse();
            AddHandler();

            TextDisplay.Visibility = Visibility.Hidden;
            InputValue.Visibility = Visibility.Visible;
            InputValue.Focus();
            InputValue.SelectAll();
            Text = InputValue.Text;
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            TextDisplay.Text = InputValue.Text;
            TextDisplay.Visibility = Visibility.Visible;
            InputValue.Visibility = Visibility.Hidden;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            this.ReleaseMouseCapture();
            Keyboard.ClearFocus();
        }

        private void AddHandler()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseDownOutsideElement), true);
        }

        //private DependencyObject GetDpObjectFromVisualTree(DependencyObject startObject, Type type)
        //{
        //    DependencyObject parent = startObject;
        //    while (parent != null)
        //    {
        //        if (type.IsInstanceOfType(parent))
        //            break;
        //        else
        //            parent = VisualTreeHelper.GetParent(parent);
        //    }
        //    return parent;
        //}

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        private void TextInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
