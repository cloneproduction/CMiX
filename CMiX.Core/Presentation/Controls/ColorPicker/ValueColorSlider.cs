// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;

namespace CMiX.Core.Presentation.Controls
{
    public class ValueColorSlider : ColorSlider
    {
        public ValueColorSlider()
        {
            Minimum = 0;
            Maximum = 100;
        }

        protected override void OnValueChanged()
        {
            base.OnValueChanged();
            ColorManager.Color.HSV_V = Value;
        }

        protected override void ColorManager_ColorChanged(Color obj)
        {
            base.ColorManager_ColorChanged(obj);
            Value = ColorManager.Color.HSV_V;
        }
    }
}
