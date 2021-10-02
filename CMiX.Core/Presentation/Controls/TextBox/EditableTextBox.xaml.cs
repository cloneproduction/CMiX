// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public partial class EditableTextBox : UserControl
    {
        public EditableTextBox()
        {
            InitializeComponent();
            InputValue.Visibility = Visibility.Hidden;
            TextDisplay.Visibility = Visibility.Visible;
            this.PreviewMouseDoubleClick += EditableTextBox_MouseDoubleClick;
            this.PreviewMouseLeftButtonDown += EditableTextBox_PreviewMouseDown;
        }

        private void EditableTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var poeut = Mouse.DirectlyOver;
            //e.Handled = true;
        }

        private void EditableTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //((IInputElement)sender).CaptureMouse();
            this.OnSwitchToEditingMode();
            //this.InputValue.CaptureMouse();
            e.Handled = true;
        }


        #region PROPERTIES
        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableTextBox), new UIPropertyMetadata(false, new PropertyChangedCallback(IsEditing_PropertyChanged)));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set
            {
                SetValue(IsEditingProperty, value);
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register("IsSelected", typeof(bool), typeof(EditableTextBox));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsEditing_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EditableTextBox textbox = d as EditableTextBox;
            textbox.OnSwitchToEditingMode();
            textbox.InputValue.Focus();
            textbox.InputValue.SelectAll();
        }



        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBox), new UIPropertyMetadata(new PropertyChangedCallback(TextProperty_PropertyChanged)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }





        private static void TextProperty_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            EditableTextBox textbox = obj as EditableTextBox;
            var newtext = (string)e.NewValue;
            textbox.TextDisplay.Text = newtext;
            textbox.InputValue.Text = newtext;
        }


        public Window _ParentItemsControl { get; set; }
        #endregion

        #region EVENTS
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
            base.OnLostFocus(e);
            OnSwitchToNormalMode();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            OnSwitchToNormalMode();
        }

        public void OnMouseDownOutsideElement(object sender, MouseButtonEventArgs e)
        {

            OnSwitchToNormalMode();

            e.Handled = true;
            ((IInputElement)sender).ReleaseMouseCapture();
        }
        #endregion

        #region PRIVATE METHODS
        private void OnSwitchToEditingMode()
        {
            TextDisplay.Visibility = Visibility.Collapsed;
            InputValue.Visibility = Visibility.Visible;
            HookItemsControlEvents();
            InputValue.CaptureMouse();

            Text = InputValue.Text;
            InputValue.Focus();
            InputValue.SelectAll();
        }



        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            //RemoveHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseDownOutsideElement));
            //IsEditing = false;
            TextDisplay.Text = InputValue.Text;
            TextDisplay.Visibility = Visibility.Visible;
            InputValue.Visibility = Visibility.Hidden;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(InputValue), null);
            Keyboard.ClearFocus();
        }

        private void HookItemsControlEvents()
        {
            //this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler((s, e) => this.OnSwitchToNormalMode()));
            //Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            _ParentItemsControl = this.GetDpObjectFromVisualTree(this, typeof(Window)) as Window;
            if (_ParentItemsControl != null)
            {
                //_ParentItemsControl.Cursor = Cursors.IBeam;
                _ParentItemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler((s, e) => this.OnSwitchToNormalMode()), true);
                //_ParentItemsControl.MouseDown += new MouseButtonEventHandler((s, e) => this.OnSwitchToNormalMode());
                _ParentItemsControl.MouseDown += _ParentItemsControl_MouseDown;
                _ParentItemsControl.SizeChanged += new SizeChangedEventHandler((s, e) => this.OnSwitchToNormalMode());
            }
        }

        private void _ParentItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnSwitchToNormalMode();
            e.Handled = true;
            //((FrameworkElement)sender).Cursor = Cursors.Arrow;
        }

        private DependencyObject GetDpObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
        #endregion
    }
}
