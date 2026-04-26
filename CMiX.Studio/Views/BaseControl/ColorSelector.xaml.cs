using System;
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
            Loaded += (s, e) =>
            {
                colorPickerPopup.PlacementTarget = PopupToggle;
                PopupToggle.Checked += (s2, e2) => colorPickerPopup.IsOpen = true;
                PopupToggle.Unchecked += (s2, e2) => colorPickerPopup.IsOpen = false;
            };

            PopupToggle.Checked += (s2, e2) =>
            {
                colorPickerPopup.IsOpen = true;
            };
            PopupToggle.Unchecked += (s2, e2) =>
            {
                colorPickerPopup.IsOpen = false;
            };
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
    }
}
