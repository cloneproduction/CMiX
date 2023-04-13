// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.PostFX
{
    public class AmbientOcclusionModel : IModel
    {
        public AmbientOcclusionModel()
        {
            ID = Guid.NewGuid();

            IsEnabled = new BooleanValueModel();
            Samples = new IntegerValueModel(13);
            ProjectionScale = new FloatValueModel(0.5f);
            Intensity = new FloatValueModel(0.2f);
            SampleBias = new FloatValueModel(0.01f);
            SampleRadius = new FloatValueModel(1.0f);
            BlurCount = new IntegerValueModel(2);
            BlurRadius = new FloatValueModel(1.85f);
            EdgeSharpness = new FloatValueModel(3.0f);
        }

        public Guid ID { get; set; }

        public BooleanValueModel IsEnabled { get; set; }
        public IntegerValueModel Samples { get; set; }
        public FloatValueModel ProjectionScale { get; set; }
        public FloatValueModel Intensity { get; set; }
        public FloatValueModel SampleBias { get; set; }
        public FloatValueModel SampleRadius { get; set; }
        public IntegerValueModel BlurCount { get; set; }
        public FloatValueModel BlurRadius { get; set; }
        public FloatValueModel EdgeSharpness { get; set; }

    }
}
