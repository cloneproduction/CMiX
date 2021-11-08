// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    [TemplatePart(Name = PART_ColorWheel, Type = typeof(ColorWheel))]
    [TemplatePart(Name = PART_AlphaSlider, Type = typeof(RgbaColorSlider))]
    [TemplatePart(Name = PART_RSlider, Type = typeof(RgbaColorSlider))]
    [TemplatePart(Name = PART_GSlider, Type = typeof(RgbaColorSlider))]
    [TemplatePart(Name = PART_BSlider, Type = typeof(RgbaColorSlider))]
    [TemplatePart(Name = PART_HSlider, Type = typeof(HueColorSlider))]
    [TemplatePart(Name = PART_SSlider, Type = typeof(SaturationColorSlider))]
    [TemplatePart(Name = PART_VSlider, Type = typeof(ValueColorSlider))]

    public class ColorPicker : Control//, IColorClient
    {

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        private const string PART_ColorWheel = "PART_ColorWheel";

        private const string PART_AlphaSlider = "PART_AlphaSlider";

        private const string PART_RSlider = "PART_RSlider";
        private const string PART_GSlider = "PART_GSlider";
        private const string PART_BSlider = "PART_BSlider";

        private const string PART_HSlider = "PART_HSlider";
        private const string PART_SSlider = "PART_SSlider";
        private const string PART_VSlider = "PART_VSlider";

        private ColorWheel _colorWheel;

        private RgbaColorSlider _rSlider;
        private RgbaColorSlider _gSlider;
        private RgbaColorSlider _bSlider;

        private HueColorSlider _hSlider;
        private SaturationColorSlider _sSlider;
        private ValueColorSlider _vSlider;


        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
            new FrameworkPropertyMetadata(Colors.Red,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnSelectedColorChanged));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }


        private IColorManager _colorManager = new ColorManager();
        private bool _lock;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            var alphaSlider = GetTemplateChild(PART_AlphaSlider) as RgbaColorSlider;

            _colorWheel = GetTemplateChild(PART_ColorWheel) as ColorWheel;
            _rSlider = GetTemplateChild(PART_RSlider) as RgbaColorSlider;
            _gSlider = GetTemplateChild(PART_GSlider) as RgbaColorSlider;
            _bSlider = GetTemplateChild(PART_BSlider) as RgbaColorSlider;

            _hSlider = GetTemplateChild(PART_HSlider) as HueColorSlider;
            _sSlider = GetTemplateChild(PART_SSlider) as SaturationColorSlider;
            _vSlider = GetTemplateChild(PART_VSlider) as ValueColorSlider;

            _colorManager?.AddClient(
                alphaSlider,
                _colorWheel,
                _rSlider,
                _gSlider,
                _bSlider,
                _hSlider,
                _sSlider,
                _vSlider);

            _colorManager.ColorChanged += Manager_ColorChanged;
        }

        private void Manager_ColorChanged(Color color)
        {
            _lock = true;

            SelectedColor = color;

            _lock = false;
        }

        private void ColorChanged(Color color)
        {
            if (_lock)
                return;

            _colorManager.ColorChanged -= Manager_ColorChanged;

            //_colorManager.CurrentColor = color.ToColor();

            _colorManager.ColorChanged += Manager_ColorChanged;
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker colorPicker)
                colorPicker.ColorChanged(colorPicker.SelectedColor);
        }
    }
}
