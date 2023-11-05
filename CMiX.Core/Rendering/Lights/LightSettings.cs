// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public class LightSettings
    {
        public LightSettings(ColorValue lightColor, Vector3 position, Vector3 target, FloatValue radius, FloatValue angle, FloatValue softness, FloatValue intensity, GenericValue<LightType> lightTypeSelector) 
        {
            LightColor = lightColor; // new ColorValue();
            Position = position;// new Vector3(0.0f, 2.0f, 0.0f);
            Target = target;// new Vector3(0.001f, 0.0f, 0.0f);
            Radius = radius;// new FloatValue(5.0f);
            Angle = angle;// new FloatValue(0.25f);
            Softness = softness;// new FloatValue(0.01f);
            Intensity = intensity;// new FloatValue(1.0f);
            LightTypeSelector = lightTypeSelector;// new GenericValue<LightType>(LightType.AmbientLight);
        }

        public GenericValue<LightType> LightTypeSelector { get; set; }
        public ColorValue LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Angle { get; set; }
        public FloatValue Softness { get; set; }
        public FloatValue Intensity { get; set; }
    }
}
