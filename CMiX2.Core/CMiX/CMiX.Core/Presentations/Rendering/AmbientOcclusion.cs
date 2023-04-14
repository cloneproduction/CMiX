// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.PostFX;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class AmbientOcclusion : ObservableObject, IControl
    {
        public AmbientOcclusion(AmbientOcclusionModel ambientOcclusionModel, CompositionService compositionService)
        {
            ID = ambientOcclusionModel.ID;
            IsEnabled = new BooleanValue(ambientOcclusionModel.IsEnabled, compositionService);
            Samples = new IntegerValue(ambientOcclusionModel.Samples, compositionService);
            ProjectionScale = new FloatValue(ambientOcclusionModel.ProjectionScale, compositionService);
            Intensity = new FloatValue(ambientOcclusionModel.Intensity, compositionService);
            SampleBias = new FloatValue(ambientOcclusionModel.SampleBias, compositionService);
            SampleRadius = new FloatValue(ambientOcclusionModel.SampleRadius, compositionService);
            BlurCount = new IntegerValue(ambientOcclusionModel.BlurCount, compositionService);
            BlurRadius = new FloatValue(ambientOcclusionModel.BlurRadius, compositionService);
            EdgeSharpness = new FloatValue(ambientOcclusionModel.EdgeSharpness, compositionService);
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
