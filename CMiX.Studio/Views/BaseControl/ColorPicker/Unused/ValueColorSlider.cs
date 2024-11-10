//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System.Windows;
//using System.Windows.Media;

//namespace CMiX.Studio.Views.BaseControl
//{
//    public class ValueColorSlider : CMiXSlider, IColorClient
//    {
//        static ValueColorSlider()
//        {
//            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValueColorSlider), new FrameworkPropertyMetadata(typeof(ValueColorSlider)));
//        }

//        public ValueColorSlider()
//        {
//            Minimum = 0;
//            Maximum = 100;
//            ValueChanged += ColorSlider_ValueChanged;
//        }

//        private void OnValueChanged()
//        {
//            ColorManager.Color.HSV_V = Value;
//        }

//        private void ColorManager_ColorChanged(Color obj)
//        {
//            Value = ColorManager.Color.HSV_V;
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
//    }
//}
