// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering
{
    public class AmbientOcclusionModel : IControlModel
    {
        public AmbientOcclusionModel()
        {
            ID = Guid.NewGuid();

            IsEnabled = new GenericValueModel<bool>(false);
            Samples = new GenericValueModel<int>(13);
            ProjectionScale = new GenericValueModel<float>(0.5f);
            Intensity = new GenericValueModel<float>(0.2f);
            SampleBias = new GenericValueModel<float>(0.01f);
            SampleRadius = new GenericValueModel<float>(1.0f);
            BlurCount = new GenericValueModel<int>(2);
            BlurRadius = new GenericValueModel<float>(1.85f);
            EdgeSharpness = new GenericValueModel<float>(3.0f);
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> IsEnabled { get; set; }
        public GenericValueModel<int> Samples { get; set; }
        public GenericValueModel<float> ProjectionScale { get; set; }
        public GenericValueModel<float> Intensity { get; set; }
        public GenericValueModel<float> SampleBias { get; set; }
        public GenericValueModel<float> SampleRadius { get; set; }
        public GenericValueModel<int> BlurCount { get; set; }
        public GenericValueModel<float> BlurRadius { get; set; }
        public GenericValueModel<float> EdgeSharpness { get; set; }

    }
}
