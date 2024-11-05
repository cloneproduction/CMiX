using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMiX.Studio.Views.BaseControl
{
    public partial class ColorSelector : UserControl
    {
        public ColorSelector()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty PositionProperty =
        DependencyProperty.Register("Position", typeof(ControlPosition), typeof(ColorSelector), new FrameworkPropertyMetadata(ControlPosition.Default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ControlPosition Position
        {
            get { return (ControlPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register("Caption", typeof(string), typeof(ColorSelector), new FrameworkPropertyMetadata(String.Empty));
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSelector), new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 255, 255), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (colorPickerPopup.IsOpen == true)
                colorPickerPopup.IsOpen = false;
            else colorPickerPopup.IsOpen = true;
            e.Handled = true;
        }

        private void colorPickerPopup_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(popupBorder);

            if (mousePosition.X < -16 ||
                mousePosition.Y > popupBorder.ActualHeight + 16 ||
                mousePosition.Y < -16 ||
                mousePosition.X > popupBorder.ActualWidth + +16)
            {
                colorPickerPopup.IsOpen = false;
            }
        }

        private void colorPickerPopup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
    }
}
