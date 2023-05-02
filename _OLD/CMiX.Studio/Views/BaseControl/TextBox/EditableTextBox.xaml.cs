// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
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

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;

            return FindParent<T>(parentObject);
        }

        private void EditableTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement != null)
            {
                var item = FindParent<ListBoxItem>(frameworkElement);
                if (item != null)
                    item.IsSelected = true;
            }
        }

        private void EditableTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.OnSwitchToEditingMode();
            }
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
            OnSwitchToNormalMode();
            base.OnLostFocus(e);

        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            OnSwitchToNormalMode();
            base.OnLostKeyboardFocus(e);
        }

        public void OnMouseDownOutsideElement(object sender, MouseButtonEventArgs e)
        {

            OnSwitchToNormalMode();

            //e.Handled = true;

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

            //InputValue.ReleaseMouseCapture();
            //FocusManager.SetFocusedElement(FocusManager.GetFocusScope(InputValue), null);
            //Keyboard.ClearFocus();
        }

        private void HookItemsControlEvents()
        {
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
