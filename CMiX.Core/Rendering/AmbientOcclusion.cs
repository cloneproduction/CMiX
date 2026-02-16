// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.ViewModels
{
    public partial class AmbientOcclusion : ObservableObject, IControl
    {
        public AmbientOcclusion(GenericValue<bool> isEnable, 
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
            Samples = samples;
            ProjectionScale = projectionScale;
            Intensity = intensity;
            SampleBias = sampleBias;
            SampleRadius = sampleRadius;
            BlurCount = blurCount;
            BlurRadius = blurRadius;
            EdgeSharpness = edgeSharpness;
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

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
