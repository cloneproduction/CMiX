// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
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
        }

        public event EventHandler MyCustomClickEvent;


        //This method is used to raise the event, when the event should be raised, 
        //this method will check to see if there are any subscribers, if there are, 
        //it raises the event
        protected virtual void OnMyCustomClickEvent(EventArgs e)
        {
            // Here, you use the "this" so it's your own control. You can also
            // customize the EventArgs to pass something you'd like.

            if (MyCustomClickEvent != null)
                MyCustomClickEvent(this, e);
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



        [Bindable(true)]
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
                Console.WriteLine("KeyDown");
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
        }
        #endregion

        #region PRIVATE METHODS
        private void OnSwitchToEditingMode()
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            //AddHandler();
            
            TextDisplay.Visibility = Visibility.Hidden;
            InputValue.Visibility = Visibility.Visible;
            HookItemsControlEvents();
            Text = InputValue.Text;
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            TextDisplay.Text = InputValue.Text;
            TextDisplay.Visibility = Visibility.Visible;
            InputValue.Visibility = Visibility.Hidden;
            Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            Mouse.Capture(null);
            Keyboard.ClearFocus();
        }

        private void HookItemsControlEvents()
        {
            //CaptureMouse();
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            _ParentItemsControl = this.GetDpObjectFromVisualTree(this, typeof(Window)) as Window;
            if (_ParentItemsControl != null)
            {
                // Handle events on parent control and determine whether to switch to Normal mode or stay in editing mode
                //_ParentItemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(this.OnScrollViewerChanged));
                _ParentItemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler((s, e) => this.OnSwitchToNormalMode()), true);

                _ParentItemsControl.MouseDown += new MouseButtonEventHandler((s, e) => this.OnSwitchToNormalMode());
                _ParentItemsControl.SizeChanged += new SizeChangedEventHandler((s, e) => this.OnSwitchToNormalMode());

                // Restrict text box to visible area of scrollviewer
                //this.ParentScrollViewer = this.GetDpObjectFromVisualTree(_ParentItemsControl, typeof(ScrollViewer)) as ScrollViewer;

                //if (this.ParentScrollViewer == null)
                //    this.ParentScrollViewer = FindVisualChild<ScrollViewer>(_ParentItemsControl);

                //Debug.Assert(this.ParentScrollViewer != null, "DEBUG ISSUE: No ScrollViewer found.");

                //if (this.ParentScrollViewer != null)
                //    _TextBox.MaxWidth = this.ParentScrollViewer.ViewportWidth;
            }
        }

        private void AddHandler()
        {
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseDownOutsideElement), true);
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
