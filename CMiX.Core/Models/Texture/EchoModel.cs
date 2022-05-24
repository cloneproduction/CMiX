// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class EchoModel : ITextureFilterModel
    {
        public EchoModel()
        {
            ID = Guid.NewGuid();
            Visible = new ToggleButtonModel(true);
            Factor = new SliderModel(0.9f);
            Name = TextureFilterName.Echo;
        }

        public ToggleButtonModel Visible { get; set ; }
        public bool Enabled { get; set ; }
        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public SliderModel Factor { get; set; }
    }
}
