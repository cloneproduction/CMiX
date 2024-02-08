// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public class LightSettings
    {
        public LightSettings(GenericValue<string> lightColor, Vector3 position, Vector3 target, GenericValue<float> radius, GenericValue<float> angle, GenericValue<float> softness, GenericValue<float> intensity, GenericValue<LightType> lightTypeSelector) 
        {
            LightColor = lightColor; // new ColorValue();
            Position = position;// new Vector3(0.0f, 2.0f, 0.0f);
            Target = target;// new Vector3(0.001f, 0.0f, 0.0f);
            Radius = radius;// new GenericValue<float>(5.0f);
            Angle = angle;// new GenericValue<float>(0.25f);
            Softness = softness;// new GenericValue<float>(0.01f);
            Intensity = intensity;// new GenericValue<float>(1.0f);
            LightTypeSelector = lightTypeSelector;// new GenericValue<LightType>(LightType.AmbientLight);
        }

        public GenericValue<LightType> LightTypeSelector { get; set; }
        public GenericValue<string> LightColor { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Angle { get; set; }
        public GenericValue<float> Softness { get; set; }
        public GenericValue<float> Intensity { get; set; }
    }
}
