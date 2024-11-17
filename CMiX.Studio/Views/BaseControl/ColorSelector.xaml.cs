using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
namespace CMiX.Studio.Views.BaseControl
{
    public partial class ColorSelector : UserControl
    {
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(ControlPosition), typeof(ColorSelector),
                new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(ColorSelector),
            new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelector),
            new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }


        public ColorSelector()
        {
            InitializeComponent();
        }

        Popup popup;
        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild("colorPickerPopup") as Popup;

            if (colorPickerPopup != null)
            {
                colorPickerPopup.MouseLeave += ColorPickerPopup_MouseLeave;
                colorPickerPopup.MouseEnter += ColorPickerPopup_MouseEnter;
            }
        }


        private void AddParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Mouse.AddPreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.AddPreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.AddPreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }

        private void RemoveParentWindowHandlers()
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                Mouse.RemovePreviewMouseDownHandler(parentWindow, ParentWindow_OnMouseDown);
                Mouse.RemovePreviewMouseUpHandler(parentWindow, ParentWindow_OnMouseUp);
                Mouse.RemovePreviewMouseMoveHandler(parentWindow, ParentWindow_OnMouseMove);
            }
        }


        private void ColorPickerPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            AddParentWindowHandlers();
        }

        private void ColorPickerPopup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(popupBorder);

            if (mousePosition.X < 0 ||
                mousePosition.Y > popupBorder.ActualHeight ||
                mousePosition.Y < 0 ||
                mousePosition.X > popupBorder.ActualWidth)
            {
                colorPickerPopup.IsOpen = false;
            }

            e.Handled = true;
        }


        private void ParentWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            colorPickerPopup.IsOpen = false;
        }

        private void ParentWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(popupBorder);

            if (mousePosition.X < -64 ||
                mousePosition.Y > popupBorder.ActualHeight + 64 ||
                mousePosition.Y < -64 ||
                mousePosition.X > popupBorder.ActualWidth + 64)
            {
                colorPickerPopup.IsOpen = false;
                RemoveParentWindowHandlers();
            }

            e.Handled = true;
        }

        private void ParentWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            RemoveParentWindowHandlers();
            e.Handled = true;
        }
    }
}
