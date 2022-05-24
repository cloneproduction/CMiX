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
            IsEnabled = new ToggleButton(ambientOcclusionModel.IsEnabled);
            Samples = new Counter(ambientOcclusionModel.Samples);
            ProjectionScale = new Slider(nameof(ProjectionScale), ambientOcclusionModel.ProjectionScale);
            Intensity = new Slider(nameof(Intensity), ambientOcclusionModel.Intensity);
            SampleBias = new Slider(nameof(SampleBias), ambientOcclusionModel.SampleBias);
            SampleRadius = new Slider(nameof(SampleRadius), ambientOcclusionModel.SampleRadius);
            BlurCount = new Counter(ambientOcclusionModel.BlurCount);
            BlurRadius = new Slider(nameof(BlurRadius), ambientOcclusionModel.BlurRadius);
            EdgeSharpness = new Slider(nameof(EdgeSharpness), ambientOcclusionModel.EdgeSharpness);
        }


        public Guid ID { get; set; }

        public ToggleButton IsEnabled { get; set; }
        public Counter Samples { get; set; }
        public Slider ProjectionScale { get; set; }
        public Slider Intensity { get; set; }
        public Slider SampleBias { get; set; }
        public Slider SampleRadius { get; set; }
        public Counter BlurCount { get; set; }
        public Slider BlurRadius { get; set; }
        public Slider EdgeSharpness { get; set; }

        public IModel GetModel()
        {
            AmbientOcclusionModel ambientOcclusionModel = new AmbientOcclusionModel();

            ambientOcclusionModel.ID = ID;
            ambientOcclusionModel.IsEnabled = (ToggleButtonModel)IsEnabled.GetModel();
            ambientOcclusionModel.Samples = (CounterModel)Samples.GetModel();
            ambientOcclusionModel.ProjectionScale = (SliderModel)ProjectionScale.GetModel();
            ambientOcclusionModel.Intensity = (SliderModel)Intensity.GetModel();
            ambientOcclusionModel.SampleBias = (SliderModel)SampleBias.GetModel();
            ambientOcclusionModel.BlurCount = (CounterModel)BlurCount.GetModel();
            ambientOcclusionModel.BlurRadius = (SliderModel)BlurRadius.GetModel();
            ambientOcclusionModel.EdgeSharpness = (SliderModel)EdgeSharpness.GetModel();

            return ambientOcclusionModel;
        }

        public void SetViewModel(IModel model)
        {
            AmbientOcclusionModel ambientOcclusionModel = model as AmbientOcclusionModel;

            this.ID = ambientOcclusionModel.ID;
            this.IsEnabled.SetViewModel(ambientOcclusionModel.IsEnabled);
            this.Samples.SetViewModel(ambientOcclusionModel.Samples);
            this.ProjectionScale.SetViewModel(ambientOcclusionModel.ProjectionScale);
            this.Intensity.SetViewModel(ambientOcclusionModel.Intensity);
            this.SampleBias.SetViewModel(ambientOcclusionModel.SampleBias);
            this.BlurCount.SetViewModel(ambientOcclusionModel.BlurCount);
            this.BlurRadius.SetViewModel(ambientOcclusionModel.BlurRadius);
            this.EdgeSharpness.SetViewModel(ambientOcclusionModel.EdgeSharpness);
        }
    }
}
