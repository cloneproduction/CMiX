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
            Radius = new FloatValueModel(1.0f);
            Brightness = new FloatValueModel(1.0f);
            Visible = new BooleanValueModel(true);
            Control = new FloatValueModel(1.0f);
        }

        public Guid ID { get; set; }

        public BooleanValueModel Visible { get; set; }
        public FloatValueModel Radius { get; set; }
        public FloatValueModel Brightness { get; set; }
        public FloatValueModel Control { get; set; }
        public TextureFilterName Name { get; internal set; }
    }
}
