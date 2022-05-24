// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Models
{
    public class AmbientOcclusionModel : IModel
    {
        public AmbientOcclusionModel()
        {
            ID = Guid.NewGuid();

            IsEnabled = new ToggleButtonModel();
            Samples = new CounterModel(13);
            ProjectionScale = new SliderModel(0.5f);
            Intensity = new SliderModel(0.2f);
            SampleBias = new SliderModel(0.01f);
            SampleRadius = new SliderModel(1.0f);
            BlurCount = new CounterModel(2);
            BlurRadius = new SliderModel(1.85f);
            EdgeSharpness = new SliderModel(3.0f);
        }


        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ToggleButtonModel IsEnabled { get; set; }
        public CounterModel Samples { get; set; }
        public SliderModel ProjectionScale { get; set; }
        public SliderModel Intensity { get; set; }
        public SliderModel SampleBias { get; set; }
        public SliderModel SampleRadius { get; set; }
        public CounterModel BlurCount { get; set; }
        public SliderModel BlurRadius { get; set; }
        public SliderModel EdgeSharpness { get; set; }

    }
}
