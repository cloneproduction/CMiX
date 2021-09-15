// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class CMiXListBoxItem : ListBoxItem
    {
        public CMiXListBoxItem()
        {
            DefaultStyleKey = typeof(CMiXListBoxItem);
            this.MouseDoubleClick += CMiXListBoxItem_MouseDoubleClick;
            this.PreviewMouseLeftButtonDown += CMiXListBoxItem_MouseDown;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }



        public ListBox _ParentItemsControl { get; set; }
        private void HookItemsControlEvents()
        {
            //this.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler((s, e) => this.OnSwitchToNormalMode()));
            //Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            _ParentItemsControl = this.GetDpObjectFromVisualTree(this, typeof(ListBox)) as ListBox;
            if (_ParentItemsControl != null)
            {
                _ParentItemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler((s, e) => this.OnSwitchToNormalMode()), true);
                _ParentItemsControl.MouseDown += new MouseButtonEventHandler((s, e) => this.OnSwitchToNormalMode());
                _ParentItemsControl.SizeChanged += new SizeChangedEventHandler((s, e) => this.OnSwitchToNormalMode());
            }
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


        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
        }

        private void CMiXListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            this.IsEditing = false;
        }

        private void CMiXListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this, CaptureMode.SubTree);


            //listBoxItem.SetCurrentValue(InputVisibilityProperty, Visibility.Collapsed);
            //listBoxItem.SetCurrentValue(DisplayVisibilityProperty, Visibility.Collapsed);
            //this.IsEditing = true;
            //HookItemsControlEvents();
        }


        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(CMiXListBoxItem), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(CMiXListBoxItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public static readonly DependencyProperty InputVisibilityProperty =
        DependencyProperty.Register("InputVisibility", typeof(Visibility), typeof(EditableTextBox), new FrameworkPropertyMetadata(Visibility.Collapsed, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Visibility InputVisibility
        {
            get { return (Visibility)GetValue(InputVisibilityProperty); }
            set { SetValue(InputVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DisplayVisibilityProperty =
        DependencyProperty.Register("DisplayVisibility", typeof(Visibility), typeof(EditableTextBox), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Visibility DisplayVisibility
        {
            get { return (Visibility)GetValue(DisplayVisibilityProperty); }
            set { SetValue(DisplayVisibilityProperty, value); }
        }



        private static void IsEditing_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("IsEditing = " + e.NewValue);
            //CMiXListBoxItem listBoxItem = d as CMiXListBoxItem;
            //listBoxItem.OnSwitchToEditingMode();
            //textbox.InputValue.Focus();
            //textbox.InputValue.SelectAll();
        }

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set
            {
                SetValue(IsEditingProperty, value);
            }
        }


        private void OnSwitchToEditingMode()
        {
            Mouse.Capture(this, CaptureMode.SubTree);

            //TextDisplay.Visibility = Visibility.Hidden;
            //InputValue.Visibility = Visibility.Visible;
            //HookItemsControlEvents();
            //Text = InputValue.Text;
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            //TextDisplay.Text = InputValue.Text;
            //TextDisplay.Visibility = Visibility.Visible;
            //InputValue.Visibility = Visibility.Hidden;

            //Mouse.Capture(null);
            //Keyboard.ClearFocus();
        }
    }
}
