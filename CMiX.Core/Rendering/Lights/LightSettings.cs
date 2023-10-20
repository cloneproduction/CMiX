// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public class LightSettings
    {
        public LightSettings() 
        {
            LightColor = new ColorValue();
            Position = new Vector3(0.0f, 2.0f, 0.0f);
            Target = new Vector3(0.001f, 0.0f, 0.0f);
            Radius = new FloatValue(5.0f);
            Angle = new FloatValue(0.25f);
            Softness = new FloatValue(0.01f);
            Intensity = new FloatValue(1.0f);
            LightTypeSelector = new GenericValue<LightType>(LightType.AmbientLight);
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
