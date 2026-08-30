// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Transformation
{
    public record TransformSRTModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<float> Uniform { get; set; } = new(1.0f);
        public Vector3Model Scale { get; set; } = new(1.0f, 1.0f, 1.0f);
        public Vector3Model Translate { get; set; } = new(0.0f, 0.0f, 0.0f);
        public Vector3Model Rotation { get; set; } = new(0.0f, 0.0f, 0.0f);
    }
}
