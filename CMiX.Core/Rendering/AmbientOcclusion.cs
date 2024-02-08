// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public class AmbientOcclusion : ObservableObject, IControl
    {
        public AmbientOcclusion(
                    GenericValue<bool> isEnable, 
                    GenericValue<int> samples, 
                    GenericValue<float> projectionScale, 
                    GenericValue<float> intensity, 
                    GenericValue<float> sampleBias, 
                    GenericValue<float> sampleRadius, 
                    GenericValue<int> blurCount, 
                    GenericValue<float> blurRadius, 
                    GenericValue<float> edgeSharpness)
        {
            IsEnabled = isEnable;
            Samples = samples; // new GenericValue<int>(13);
            ProjectionScale = projectionScale; // new GenericValue<float>(0.5f);
            Intensity = intensity; // new GenericValue<float>(0.2f);
            SampleBias = sampleBias; // new GenericValue<float>(0.01f);
            SampleRadius = sampleRadius; // new GenericValue<float>(1.0f);
            BlurCount = blurCount; // new GenericValue<int>(2);
            BlurRadius = blurRadius; // new GenericValue<float>(1.85f);
            EdgeSharpness = edgeSharpness; // new GenericValue<float>(3.0f);
        }


        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<int> Samples { get; set; }
        public GenericValue<float> ProjectionScale { get; set; }
        public GenericValue<float> Intensity { get; set; }
        public GenericValue<float> SampleBias { get; set; }
        public GenericValue<float> SampleRadius { get; set; }
        public GenericValue<int> BlurCount { get; set; }
        public GenericValue<float> BlurRadius { get; set; }
        public GenericValue<float> EdgeSharpness { get; set; }
    }
}
