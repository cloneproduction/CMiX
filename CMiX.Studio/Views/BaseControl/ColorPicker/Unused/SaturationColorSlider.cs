//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Windows;
//using System.Windows.Media;

//namespace CMiX.Studio.Views.BaseControl
//{
//    public class SaturationColorSlider : CMiXSlider, IColorClient
//    {
//        static SaturationColorSlider()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(SaturationColorSlider), new FrameworkPropertyMetadata(typeof(SaturationColorSlider)));
//        }

//        public SaturationColorSlider()
//        {
//            Minimum = 0;
//            Maximum = 1;
//            ValueChanged += ColorSlider_ValueChanged;
//        }


//        protected bool UpdateBackgroundWhenColorUpdated = true;
//        protected IColorManager ColorManager { get; private set; }


//        public void Init(IColorManager colorManager)
//        {
//            ColorManager = colorManager;
//            ColorManager.ColorChanged += ColorManager_ColorChanged;
//        }


//        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//        {
//            OnValueChanged();
//        }

//        private void OnValueChanged()
//        {
//            ColorManager.Color.HSV_S = Value;
//        }

//        private void ColorManager_ColorChanged(Color obj)
//        {
//            Value = ColorManager.Color.HSV_S;
//        }
//    }
//}
