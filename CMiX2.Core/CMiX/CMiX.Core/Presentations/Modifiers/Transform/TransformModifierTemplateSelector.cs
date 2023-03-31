// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class TransformModifierTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RandomXYZTemplate { get; set; }
        public DataTemplate LinearXYZTemplate { get; set; }
        public DataTemplate LFOTemplate { get; set; }
        public DataTemplate RandomScaleTemplate { get; set; }
        public DataTemplate TransformTemplate { get; set; }
        public DataTemplate TranslateTemplate { get; set; }
        public DataTemplate ScaleTemplate { get; set; }
        public DataTemplate RotationTemplate { get; set; }
        public DataTemplate StepperTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;

            if (item != null)
            {
                if (item is RandomXYZ)
                    dataTemplate = RandomXYZTemplate;
                else if (item is LinearXYZ)
                    dataTemplate = LinearXYZTemplate;
                else if (item is LFO)
                    dataTemplate = LFOTemplate;
                else if (item is RandomScale)
                    dataTemplate = RandomScaleTemplate;
                else if (item is TransformSRT)
                    dataTemplate = TransformTemplate;
                else if (item is Translate)
                    dataTemplate = TranslateTemplate;
                else if (item is Scale)
                    dataTemplate = ScaleTemplate;
                else if (item is Rotation)
                    dataTemplate = RotationTemplate;
                else if (item is Stepper)
                    dataTemplate = StepperTemplate;
            }

            return dataTemplate;
        }
    }
}
