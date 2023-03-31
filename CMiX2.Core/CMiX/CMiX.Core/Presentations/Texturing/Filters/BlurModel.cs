// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Texturing.Filters
{
    public class BlurModel : ITextureFilterModel
    {
        public BlurModel()
        {
            ID = Guid.NewGuid();

            Name = TextureFilterName.Blur;
            Visible = new BooleanValueModel(true);
            Strength = new FloatValueModel();
        }

        public Guid ID { get; set; }
        public FloatValueModel Strength { get; set; }
        public BooleanValueModel Visible { get; set; }
        public TextureFilterName Name { get; set; }
    }
}
