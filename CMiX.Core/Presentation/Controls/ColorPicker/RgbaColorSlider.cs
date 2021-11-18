// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class RgbaColorSlider : CMiXSlider, IColorClient
    {
        static RgbaColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RgbaColorSlider), new FrameworkPropertyMetadata(typeof(RgbaColorSlider)));
        }


        public RgbaColorSlider()
        {
            Minimum = 0;
            Maximum = 255;
            ValueChanged += ColorSlider_ValueChanged;
        }


        public static readonly DependencyProperty ChannelProperty = DependencyProperty.Register(
           nameof(Channel), typeof(RgbaChannel), typeof(RgbaColorSlider),
           new PropertyMetadata(default(RgbaChannel)));


        public RgbaChannel Channel
        {
            get => (RgbaChannel)GetValue(ChannelProperty);
            set => SetValue(ChannelProperty, value);
        }


        protected bool UpdateBackgroundWhenColorUpdated = true;
        protected IColorManager ColorManager { get; private set; }


        public void Init(IColorManager colorManager)
        {
            ColorManager = colorManager;
            ColorManager.ColorChanged += ColorManager_ColorChanged;
        }


        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnValueChanged();
        }


        private void ColorManager_ColorChanged(Color obj)
        {
            switch (Channel)
            {
                case RgbaChannel.Red:
                    Value = ColorManager.Color.RGB_R;
                    break;
                case RgbaChannel.Green:
                    Value = ColorManager.Color.RGB_G;
                    break;
                case RgbaChannel.Blue:
                    Value = ColorManager.Color.RGB_B;
                    break;
                case RgbaChannel.Alpha:
                    Value = ColorManager.Color.A;
                    break;
                default:
                    break;
            }
        }


        private void OnValueChanged()
        {
            switch (Channel)
            {
                case RgbaChannel.Red:
                    ColorManager.Color.RGB_R = Value;
                    break;
                case RgbaChannel.Green:
                    ColorManager.Color.RGB_G = Value;
                    break;
                case RgbaChannel.Blue:
                    ColorManager.Color.RGB_B = Value;
                    break;
                case RgbaChannel.Alpha:
                    ColorManager.Color.A = Value;
                    break;
            }
        }
    }
}
