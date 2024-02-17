// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public class LightSettingsModel : IControlModel
    {
        public LightSettingsModel()
        {
            LightColor = new GenericValueModel<string>("#FFFF00FF");
            Position = new Vector3Model(0.0f, 2.0f, 0.0f);
            Target = new Vector3Model(0.001f, 0.0f, 0.0f);
            Radius = new GenericValueModel<float>(5.0f);
            Angle = new GenericValueModel<float>(0.25f);
            Softness = new GenericValueModel<float>(0.01f);
            Intensity = new GenericValueModel<float>(1.0f);
            LightTypeSelector = new GenericValueModel<LightType>(LightType.AmbientLight);
        }

        public Guid ID { get; set; }

        public GenericValueModel<LightType> LightTypeSelector { get; set; }
        public GenericValueModel<string> LightColor { get; set; }
        public Vector3Model Position { get; set; }
        public Vector3Model Target { get; set; }
        public GenericValueModel<float> Radius { get; set; }
        public GenericValueModel<float> Angle { get; set; }
        public GenericValueModel<float> Softness { get; set; }
        public GenericValueModel<float> Intensity { get; set; }
    }
}
