// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class AmbientOcclusion : ObservableObject, IControl
    {
        public AmbientOcclusion(AmbientOcclusionModel ambientOcclusionModel)
        {
            ID = ambientOcclusionModel.ID;
            IsEnabled = new BooleanValue(ambientOcclusionModel.IsEnabled);
            Samples = new IntegerValue(ambientOcclusionModel.Samples);
            ProjectionScale = new FloatValue(ambientOcclusionModel.ProjectionScale);
            Intensity = new FloatValue(ambientOcclusionModel.Intensity);
            SampleBias = new FloatValue(ambientOcclusionModel.SampleBias);
            SampleRadius = new FloatValue(ambientOcclusionModel.SampleRadius);
            BlurCount = new IntegerValue(ambientOcclusionModel.BlurCount);
            BlurRadius = new FloatValue(ambientOcclusionModel.BlurRadius);
            EdgeSharpness = new FloatValue(ambientOcclusionModel.EdgeSharpness);
        }


        public Guid ID { get; set; }

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
