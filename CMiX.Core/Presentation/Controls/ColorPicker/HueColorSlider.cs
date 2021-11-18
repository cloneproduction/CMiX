// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class HueColorSlider : CMiXSlider, IColorClient
    {
        static HueColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HueColorSlider), new FrameworkPropertyMetadata(typeof(HueColorSlider)));
        }

        public HueColorSlider()
        {
            Minimum = 0;
            Maximum = 359;
            UpdateBackgroundWhenColorUpdated = false;
            ValueChanged += ColorSlider_ValueChanged;
        }


        private void OnValueChanged()
        {
            ColorManager.Color.HSV_H = Value;
        }

        private void ColorManager_ColorChanged(Color obj)
        {
            Value = ColorManager.Color.HSV_H;
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
    }
}
