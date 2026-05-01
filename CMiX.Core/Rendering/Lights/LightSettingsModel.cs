// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering.Lights
{
    public record LightSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<LightType> LightTypeSelector { get; set; } = new(LightType.AmbientLight);
        public GenericValueModel<string> Grey700Brush { get; set; } = new("#FFFF00FF");
        public Vector3Model Position { get; set; } = new(0.0f, 2.0f, 0.0f);
        public Vector3Model Target { get; set; } = new(0.001f, 0.0f, 0.0f);
        public GenericValueModel<float> Radius { get; set; } = new(5.0f);
        public GenericValueModel<bool> LightHelper { get; set; } = new(false);
        public GenericValueModel<float> Angle { get; set; } = new(0.25f);
        public GenericValueModel<float> Softness { get; set; } = new(0.01f);
        public GenericValueModel<float> Intensity { get; set; } = new(1.0f);
    }
}
