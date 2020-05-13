﻿using CMiX.MVVM.Resources;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.MVVM.Controls
{
    public class CMiXSlider : Slider
    {
        public CMiXSlider()
        {
            OnApplyTemplate();
        }

        TextBox TextInput { get; set; }

        public override void OnApplyTemplate()
        {
            TextInput = GetTemplateChild("textInput") as TextBox;

            if(TextInput != null)
            {
                TextInput.Visibility = Visibility.Hidden;
                TextInput.PreviewMouseDown += TextInput_PreviewMouseDown;
            }
                

            base.OnApplyTemplate();
        }

        private void TextInput_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("TextInput_PreviewMouseDown");
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_mouseDownPos != null & IsEditing == false)
            {
                var currentPoint = GetMousePosition();
                var offset = currentPoint - _lastPoint.Value;

                newValue = this.Value + offset.X * ((Math.Abs(this.Minimum) + Math.Abs(this.Maximum)) / ActualWidth);

                if (newValue >= this.Maximum)
                    newValue = this.Maximum;
                else if (newValue <= this.Minimum)
                    newValue = this.Minimum;

                this.Value = newValue;
                _lastPoint = GetMousePosition();
                Console.WriteLine("OnPreviewMouseMove");
            }
            //base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            Console.WriteLine("OnPreviewMouseUp");
            var mouseUpPos = e.GetPosition(this);

            if (_mouseDownPos != null && IsEditing == false)
            {
                Point pointToScreen;
                double YPos = ActualHeight / 2;
                double XPos = 0;

                if (mouseUpPos.X > this.ActualWidth)
                    XPos = ActualWidth - 1;
                else if (mouseUpPos.X < 0)
                    XPos = 0;
                else
                    XPos = Utils.Map(this.Value, this.Minimum, this.Maximum, 0, ActualWidth);

                pointToScreen = this.PointToScreen(new Point(XPos, YPos));
                SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            }

            

            if (_mouseDownPos == mouseUpPos)
            {
                OnSwitchToEditingMode();// = true;
            }
            _mouseDownPos = null;
            if(!IsEditing)
                this.ReleaseMouseCapture();
            //base.OnPreviewMouseUp(e);
        }

        public bool IsEditing { get; set; }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if(IsEditing == false)
            {
                _lastPoint = GetMousePosition();
                _mouseDownPos = e.GetPosition(this);
                this.CaptureMouse();
                //base.OnPreviewMouseDown(e);
            }
            
            Console.WriteLine("OnPreviewMouseDown");
        }


        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private Point? _mouseDownPos;
        private Point? _lastPoint;
        private double newValue;

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(Slider), new PropertyMetadata(""));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

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
            e.Handled = true;
            OnSwitchToNormalMode();
            Console.WriteLine("OnLostFocus");
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
            OnSwitchToNormalMode();
        }

        public void OnMouseDownOutsideElement(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("OnMouseDownOutsideElement");
            OnSwitchToNormalMode();
            e.Handled = true;
        }
        #endregion

        private void OnSwitchToEditingMode()
        {
            //AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseDownOutsideElement), true);
            IsEditing = true;
            TextInput.Visibility = Visibility.Visible;
            TextInput.Focus();
            TextInput.SelectAll();
            this.ReleaseMouseCapture();
            if (!TextInput.IsMouseCaptured)
            {
                TextInput.CaptureMouse();
                Console.WriteLine("TextInput.CaptureMouse();");
                //this.ReleaseMouseCapture();
                //TextInput.CaptureMouse();
                //Mouse.Capture(TextInput);
                //Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            }
            if(TextInput.IsMouseCaptured)
                Console.WriteLine("TextInput MouseCaptured");

        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            TextInput.Visibility = Visibility.Hidden;

            if (TextInput.IsMouseCaptured)
            {
                TextInput.ReleaseMouseCapture();
                Mouse.RemovePreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideElement);
            }

            Keyboard.ClearFocus();
            //this.Focus();
        }

        private readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void TextInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        //public static readonly DependencyProperty IsEditingProperty =
        //DependencyProperty.Register("IsEditing", typeof(bool), typeof(CMiXSlider), new UIPropertyMetadata(false, new PropertyChangedCallback(IsEditing_PropertyChanged)));
        //public bool IsEditing
        //{
        //    get { return (bool)GetValue(IsEditingProperty); }
        //    set { SetValue(IsEditingProperty, value); }
        //}

        //private static void IsEditing_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //var textbox = d as EditableValue;
        //    //this.OnSwitchToEditingMode();
        //}
    }
}
