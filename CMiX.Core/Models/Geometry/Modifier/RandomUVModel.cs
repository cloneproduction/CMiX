// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class RandomUVModel : ITextureFilterModel
    {
        public RandomUVModel()
        {
            this.ID = Guid.NewGuid();
            Name = TextureFilterName.RandomUV;

            Visible = new ToggleButtonModel(true);

            BeatModifierModel = new BeatModifierModel();
            CounterModel = new CounterModel(1);
            EasingModel = new EasingModel();

            RandomizeLocation = new ToggleButtonModel();
            LocationX = new SliderModel();
            LocationY = new SliderModel();

            RandomizeScale = new ToggleButtonModel();
            ScaleX = new SliderModel();
            ScaleY = new SliderModel();
            Uniform = new SliderModel();

            RandomizeRotation = new ToggleButtonModel();
            Rotation = new SliderModel();

            SamplerState = new SamplerStateModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }



        public ToggleButtonModel Visible { get; set; }

        public EasingModel EasingModel { get; set; }
        public CounterModel CounterModel { get; set; }

        public ToggleButtonModel RandomizeLocation { get; set; }
        public SliderModel LocationX { get; set; }
        public SliderModel LocationY { get; set; }

        public ToggleButtonModel RandomizeScale { get; set; }
        public SliderModel ScaleX { get; set; }
        public SliderModel ScaleY { get; set; }
        public SliderModel Uniform { get; set; }

        public ToggleButtonModel RandomizeRotation { get; set; }
        public SliderModel Rotation { get; set; }


        public BeatModifierModel BeatModifierModel { get; set; }
        public TextureFilterName Name { get; set; }

        public int Count { get; set; }
        public ToggleButtonModel Spread { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
    }
}
