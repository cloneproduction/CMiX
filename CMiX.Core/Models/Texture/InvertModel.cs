// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class InvertModel : ITextureFilterModel
    {
        public InvertModel()
        {
            ID = Guid.NewGuid();

            Name = TextureFilterName.Invert;
            Factor = new SliderModel(1.0f);
            InvertChannelSelector = new ComboBoxModel<InvertChannel>(InvertChannel.Value);
            InvertAlpha = new ToggleButtonModel();
            Visible = new ToggleButtonModel(true);
            Control = new SliderModel();

            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Factor { get; set; }
        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<InvertChannel> InvertChannelSelector { get; set; }
        public TextureFilterName Name { get; set; }
        public ToggleButtonModel InvertAlpha { get; internal set; }
        public SliderModel Control { get; set; }
    }
}
