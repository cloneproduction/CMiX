using CMiX.Core.Mathematics;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class DragValue : UserControl
    {
        public DragValue()
        {
            InitializeComponent();
            OnApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            if (borderValueDisplay != null)
            {
                borderValueDisplay.PreviewMouseLeftButtonDown += Border_PreviewMouseLeftButtonDown;
                borderValueDisplay.PreviewMouseUp += Border_PreviewMouseUp;
                borderValueDisplay.PreviewMouseMove += Border_PreviewMouseMove;
            }

            if (ValueInput != null)
            {
                ValueInput.MouseLeave += View_OnMouseLeave;
                ValueInput.MouseEnter += View_OnMouseEnter;
            }

            if (SubButton != null)
            {
                AddButton.Click += AddButton_Click;
                SubButton.Click += SubButton_Click;
            }

            ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            base.OnApplyTemplate();
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEditing == false)
            {
                _lastPoint = GetMousePosition();
                _mouseDownPos = e.GetPosition(this);
                borderValueDisplay.CaptureMouse();
            }
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownPos != null)
            {
                var currentPoint = GetMousePosition();
                var offset = currentPoint - _lastPoint.Value;

                if (currentPoint.X >= ScreenWidth - 1)
                    SetCursorPos(0, Convert.ToInt32(currentPoint.Y));
                else if (currentPoint.X <= 0)
                    SetCursorPos(ScreenWidth - 1, Convert.ToInt32(currentPoint.Y));

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    newValue = this.Value + offset.X * 0.001;
                else
                    newValue = this.Value + offset.X * 0.01;

                this.Value = newValue;
                _lastPoint = GetMousePosition();
            }
        }

        //protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        //{
        //    var mouseUpPos = e.GetPosition(this);
        //    Border.ReleaseMouseCapture();

        //    if (_mouseDownPos == mouseUpPos)
        //        OnSwitchToEditingMode();

        //    if (_mouseDownPos != null && IsEditing == false)
        //    {

        //        Point pointToScreen;

        //        double YPos = ActualHeight / 2;
        //        double XPos = MathUtils.Map(this.Value, this.Minimum, this.Maximum, 0, ActualWidth);

        //        if (XPos >= ActualWidth)
        //            XPos -= 1;

        //        pointToScreen = this.PointToScreen(new Point(XPos, YPos));
        //        SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
        //    }
        //    _mouseDownPos = null;
        //}


        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mouseUpPos = e.GetPosition(this);
            borderValueDisplay.ReleaseMouseCapture();

            if (_mouseDownPos == mouseUpPos)
                OnSwitchToEditingMode();

            if (_mouseDownPos != null && IsEditing == false)
            {

                Point pointToScreen;

                double YPos = ActualHeight / 2;
                double XPos = ActualWidth / 4 - AddButton.ActualWidth;// MathUtils.Map(this.Value, 0, 1, 0, ActualWidth);

                if (XPos >= ActualWidth)
                    XPos -= 1;

                pointToScreen = borderValueDisplay.PointToScreen(new Point(XPos, YPos));
                SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            }
            _mouseDownPos = null;

            //var mouseUpPos = e.GetPosition(this);
            //borderValueDisplay.ReleaseMouseCapture();

            //if (_mouseDownPos == mouseUpPos)
            //    OnSwitchToEditingMode();

            //if (_mouseDownPos != null && IsEditing == false)
            //{
            //    Point pointToScreen = this.PointToScreen(new Point(ActualWidth / 2, ActualHeight / 2));
            //    SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
            //}

            //_mouseDownPos = null;
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            OnSwitchToNormalMode();
            CancelUpdateValue();
        }

        private void View_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
        }

        private void View_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
        }

        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);

            if (IsEditing == true)
            {
                if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
                    UpdateValue();
                else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
                    CancelUpdateValue();

                OnSwitchToNormalMode();
            }
        }

        private void TextInput_GotFocus(object sender, RoutedEventArgs e)
        {
            ValueInput.MouseLeave += View_OnMouseLeave;
            ValueInput.MouseEnter += View_OnMouseEnter;
        }

        private void Text_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ValueInput.MouseLeave -= View_OnMouseLeave;
            ValueInput.MouseEnter -= View_OnMouseEnter;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value += 0.001;
            e.Handled = true;
        }

        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value -= 0.001;
            e.Handled = true;
        }

        private int ScreenHeight;
        private int ScreenWidth;


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

        private Point? _lastPoint;
        private Point? _mouseDownPos;
        private double newValue;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateValue();
                OnSwitchToNormalMode();
            }
            else if (e.Key == Key.Escape)
            {
                CancelUpdateValue();
                OnSwitchToNormalMode();
            }

            //e.Handled = true;// !IsTextAllowed(ValueInput.Text);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            //e.Handled = !IsTextAllowed(ValueInput.Text);
        }

        private void OnSwitchToEditingMode()
        {
            IsEditing = true;
            ValueInput.Focus();
            ValueInput.SelectAll();
        }

        private void OnSwitchToNormalMode(bool bCancelEdit = true)
        {
            IsEditing = false;
            Keyboard.ClearFocus();
            this.Focus();
            _mouseDownPos = null;
        }

        private readonly Regex _regex = new Regex(@"/^-?(0|[1-9]\d*)(\.\d+)?$/"); //regex that matches disallowed text

        private bool IsTextAllowed(string text)
        {
            bool result = !_regex.IsMatch(text);
            return result;
        }

        public void UpdateValue()
        {
            if (!IsTextAllowed(ValueInput.Text))
            {
                ValueInput.Text = this.Value.ToString();
                return;
            }

            double result;
            var b = Double.TryParse(ValueInput.Text, out result);
            if (!b)
            {
                ValueInput.Text = this.Value.ToString();
                return;
            }

            this.Value = Double.Parse(ValueInput.Text);
        }

        public void CancelUpdateValue()
        {
            if (!IsTextAllowed(ValueInput.Text))
            {
                ValueInput.Text = this.Value.ToString();
                return;
            }

            double result;
            var b = Double.TryParse(ValueInput.Text, out result);
            if (!b)
            {
                ValueInput.Text = this.Value.ToString();
                return;
            }

            ValueInput.Text = this.Value.ToString();
        }

        public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register("IsEditing", typeof(bool), typeof(DragValue), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(DragValue), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(DragValue), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(DragValue), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
    }
}
