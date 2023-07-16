// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public class AmbientOcclusion : ObservableObject, IControl
    {
        public AmbientOcclusion()
        {
            IsEnabled = new BooleanValue();
            Samples = new IntegerValue(13);
            ProjectionScale = new FloatValue(0.5f);
            Intensity = new FloatValue(0.2f);
            SampleBias = new FloatValue(0.01f);
            SampleRadius = new FloatValue(1.0f);
            BlurCount = new IntegerValue(2);
            BlurRadius = new FloatValue(1.85f);
            EdgeSharpness = new FloatValue(3.0f);
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue IsEnabled { get; set; }
        public IntegerValue Samples { get; set; }
        public FloatValue ProjectionScale { get; set; }
        public FloatValue Intensity { get; set; }
        public FloatValue SampleBias { get; set; }
        public FloatValue SampleRadius { get; set; }
        public IntegerValue BlurCount { get; set; }
        public FloatValue BlurRadius { get; set; }
        public FloatValue EdgeSharpness { get; set; }
    }
}
