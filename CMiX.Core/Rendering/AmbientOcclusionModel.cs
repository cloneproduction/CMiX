// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering
{
    public record AmbientOcclusionModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<bool> IsEnabled { get; set; } = new(false);
        public GenericValueModel<int> Samples { get; set; } = new(13);
        public GenericValueModel<float> ProjectionScale { get; set; } = new(0.5f);
        public GenericValueModel<float> Intensity { get; set; } = new(0.2f);
        public GenericValueModel<float> SampleBias { get; set; } = new(0.01f);
        public GenericValueModel<float> SampleRadius { get; set; } = new(1.0f);
        public GenericValueModel<int> BlurCount { get; set; } = new(2);
        public GenericValueModel<float> BlurRadius { get; set; } = new(1.85f);
        public GenericValueModel<float> EdgeSharpness { get; set; } = new(3.0f);

    }
}
