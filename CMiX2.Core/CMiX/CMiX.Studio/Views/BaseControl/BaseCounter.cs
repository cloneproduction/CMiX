// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CMiX.Studio.Views.BaseControl
{
    //public class BaseCounter : Control
    //{
    //    public BaseCounter()
    //    {
    //        OnApplyTemplate();
    //    }

    //    private Border Border { get; set; }
    //    private System.Windows.Controls.Button SubButton { get; set; }
    //    private System.Windows.Controls.Button AddButton { get; set; }
    //    private TextBox ValueInput { get; set; }

    //    public override void OnApplyTemplate()
    //    {
    //        Border = GetTemplateChild("borderValueDisplay") as Border;
    //        ValueInput = GetTemplateChild("valueInput") as TextBox;

    //        SubButton = GetTemplateChild("SubButton") as System.Windows.Controls.Button;
    //        AddButton = GetTemplateChild("AddButton") as System.Windows.Controls.Button;

    //        if (Border != null)
    //        {
    //            Border.PreviewMouseLeftButtonDown += Border_PreviewMouseLeftButtonDown;
    //            Border.PreviewMouseUp += Border_PreviewMouseUp;
    //            Border.PreviewMouseMove += Border_PreviewMouseMove;
    //        }

    //        if (ValueInput != null)
    //        {
    //            ValueInput.MouseLeave += View_OnMouseLeave;
    //            ValueInput.MouseEnter += View_OnMouseEnter;
    //        }

    //        if (SubButton != null)
    //        {
    //            AddButton.Click += AddButton_Click;
    //            SubButton.Click += SubButton_Click;
    //        }

    //        ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
    //        ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

    //        base.OnApplyTemplate();
    //    }

    //    private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        if (IsEditing == false)
    //        {
    //            _lastPoint = GetMousePosition();
    //            _mouseDownPos = e.GetPosition(this);
    //            Border.CaptureMouse();
    //        }
    //    }

    //    private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (_mouseDownPos != null)
    //        {
    //            var currentPoint = GetMousePosition();
    //            var offset = currentPoint - _lastPoint.Value;

    //            if (currentPoint.X >= ScreenWidth - 1)
    //                SetCursorPos(0, Convert.ToInt32(currentPoint.Y));
    //            else if (currentPoint.X <= 0)
    //                SetCursorPos(ScreenWidth - 1, Convert.ToInt32(currentPoint.Y));

    //            newValue = Math.Round(this.Value + offset.X * 0.51);
    //            if (newValue <= 0)
    //                newValue = 0;
    //            this.Value = newValue;
    //            _lastPoint = GetMousePosition();
    //        }
    //    }

    //    private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        var mouseUpPos = e.GetPosition(this);
    //        Border.ReleaseMouseCapture();

    //        if (_mouseDownPos == mouseUpPos)
    //            OnSwitchToEditingMode();

    //        if (_mouseDownPos != null && IsEditing == false)
    //        {
    //            Point pointToScreen = this.PointToScreen(new Point(ActualWidth / 2, ActualHeight / 2));
    //            SetCursorPos(Convert.ToInt32(pointToScreen.X), Convert.ToInt32(pointToScreen.Y));
    //        }

    //        _mouseDownPos = null;
    //    }

    //    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    //    {
    //        OnSwitchToNormalMode();
    //        CancelUpdateValue();
    //    }

    //    private void View_OnMouseLeave(object sender, MouseEventArgs e)
    //    {
    //        Window parentWindow = Window.GetWindow(this);
    //        if (parentWindow != null)
    //            Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
    //    }

    //    private void View_OnMouseEnter(object sender, MouseEventArgs e)
    //    {
    //        Window parentWindow = Window.GetWindow(this);
    //        if (parentWindow != null)
    //            Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
    //    }

    //    private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    //    {
    //        Window parentWindow = Window.GetWindow(this);
    //        if (parentWindow != null)
    //            Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);

    //        if (IsEditing == true)
    //        {
    //            if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
    //                UpdateValue();
    //            else if (mouseButtonEventArgs.ChangedButton == MouseButton.Right)
    //                CancelUpdateValue();

    //            OnSwitchToNormalMode();
    //        }
    //    }

    //    private void TextInput_GotFocus(object sender, RoutedEventArgs e)
    //    {
    //        ValueInput.MouseLeave += View_OnMouseLeave;
    //        ValueInput.MouseEnter += View_OnMouseEnter;
    //    }

    //    private void Text_OnLostFocus(object sender, RoutedEventArgs e)
    //    {
    //        ValueInput.MouseLeave -= View_OnMouseLeave;
    //        ValueInput.MouseEnter -= View_OnMouseEnter;
    //    }

    //    private void AddButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        this.Value += StepValue;
    //        e.Handled = true;
    //    }

    //    private void SubButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        this.Value -= StepValue;
    //        e.Handled = true;
    //    }

    //    private int ScreenHeight;
    //    private int ScreenWidth;


    //    [DllImport("User32.dll")]
    //    private static extern bool SetCursorPos(int X, int Y);

    //    [DllImport("user32.dll")]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    internal static extern bool GetCursorPos(ref Win32Point pt);

    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct Win32Point
    //    {
    //        public Int32 X;
    //        public Int32 Y;
    //    };

    //    public static Point GetMousePosition()
    //    {
    //        Win32Point w32Mouse = new Win32Point();
    //        GetCursorPos(ref w32Mouse);
    //        return new Point(w32Mouse.X, w32Mouse.Y);
    //    }

    //    private Point? _lastPoint;
    //    private Point? _mouseDownPos;
    //    private double newValue;

    //    protected override void OnKeyDown(KeyEventArgs e)
    //    {
    //        if (e.Key == Key.Enter)
    //        {
    //            UpdateValue();
    //            OnSwitchToNormalMode();
    //        }
    //        else if (e.Key == Key.Escape)
    //        {
    //            CancelUpdateValue();
    //            OnSwitchToNormalMode();
    //        }
    //        e.Handled = !IsTextAllowed(ValueInput.Text);
    //    }

    //    protected override void OnLostFocus(RoutedEventArgs e)
    //    {
    //        e.Handled = !IsTextAllowed(ValueInput.Text);
    //    }

    //    private void OnSwitchToEditingMode()
    //    {
    //        IsEditing = true;
    //        ValueInput.Focus();
    //        ValueInput.SelectAll();
    //    }

    //    private void OnSwitchToNormalMode(bool bCancelEdit = true)
    //    {
    //        IsEditing = false;
    //        Keyboard.ClearFocus();
    //        this.Focus();
    //        _mouseDownPos = null;
    //    }

    //    private readonly Regex _regex = new Regex(@"[^0-9.-]+"); //regex that matches disallowed text
    //    private bool IsTextAllowed(string text)
    //    {
    //        double num;
    //        if (double.TryParse(text, out num))
    //            return !_regex.IsMatch(text);
    //        else
    //            return false;
    //    }

    //    public void UpdateValue()
    //    {
    //        if (IsTextAllowed(ValueInput.Text))
    //            this.Value = Double.Parse(ValueInput.Text);
    //    }

    //    public void CancelUpdateValue()
    //    {
    //        double oldValue = this.Value;
    //        if (IsTextAllowed(ValueInput.Text))
    //            this.Value = oldValue;
    //        ValueInput.Text = oldValue.ToString();
    //    }


    //    public static readonly DependencyProperty StepValueProperty =
    //    DependencyProperty.Register("StepValue", typeof(int), typeof(BaseCounter), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //    public int StepValue
    //    {
    //        get { return (int)GetValue(StepValueProperty); }
    //        set { SetValue(StepValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty IsEditingProperty =
    //    DependencyProperty.Register("IsEditing", typeof(bool), typeof(BaseCounter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //    public bool IsEditing
    //    {
    //        get { return (bool)GetValue(IsEditingProperty); }
    //        set { SetValue(IsEditingProperty, value); }
    //    }

    //    public static readonly DependencyProperty ValueProperty =
    //    DependencyProperty.Register("Value", typeof(double), typeof(BaseCounter), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //    public double Value
    //    {
    //        get { return (double)GetValue(ValueProperty); }
    //        set { SetValue(ValueProperty, value); }
    //    }

    //    public static readonly DependencyProperty PositionProperty =
    //    DependencyProperty.Register("Position", typeof(ControlPosition), typeof(BaseCounter), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    //    public ControlPosition Position
    //    {
    //        get { return (ControlPosition)GetValue(PositionProperty); }
    //        set { SetValue(PositionProperty, value); }
    //    }
    //}
}
