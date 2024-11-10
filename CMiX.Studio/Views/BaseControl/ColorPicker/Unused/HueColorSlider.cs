//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Windows;
//using System.Windows.Media;

//namespace CMiX.Studio.Views.BaseControl
//{
//    public class HueColorSlider : CMiXSlider, IColorClient
//    {
//        static HueColorSlider()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(HueColorSlider), new FrameworkPropertyMetadata(typeof(HueColorSlider)));
//        }

//        public HueColorSlider()
//        {
//            Minimum = 0;
//            Maximum = 359;
//            UpdateBackgroundWhenColorUpdated = false;
//            ValueChanged += ColorSlider_ValueChanged;
//        }


//        //private void OnValueChanged()
//        //{
//        //    ColorManager.Color.HSV_H = Value;
//        //}


//        protected bool UpdateBackgroundWhenColorUpdated = true;
//        protected ColorManager ColorManager { get; private set; }


//        public void Init(ColorManager colorManager)
//        {
//            ColorManager = colorManager;
//            ColorManager.ColorChanged += ColorManager_ColorChanged;
//        }

//        private void ColorManager_ColorChanged(Color obj)
//        {
//            if (ColorManager.Color.HSV_H != double.NaN)
//                Value = ColorManager.Color.HSV_H;
//        }

//        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//        {
//            ColorManager.Color.HSV_H = Value;
//            //OnValueChanged();
//        }
//    }
//}
