// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class EdgeModel : ITextureFilterModel
    {
        public EdgeModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.Edge;
            Radius = new SliderModel(1.0f);
            Brightness = new SliderModel(1.0f);
            Visible = new ToggleButtonModel(true);
            Control = new SliderModel(1.0f);
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ToggleButtonModel Visible { get; set; }
        public SliderModel Radius { get; set; }
        public SliderModel Brightness { get; set; }
        public SliderModel Control { get; set; }
        public TextureFilterName Name { get; internal set; }
    }
}
