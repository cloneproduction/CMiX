// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class SetAlpha : ObservableObject, IControl, ITextureModifier
    {
        public SetAlpha(GenericValue<bool> visible,
                        GenericValue<bool> invert,
                        GenericValue<bool> keepOriginalAlpha,
                        GenericValue<AlphaChannel> alphaChannel, 
                        GenericValue<float> control)
        {
            Visible = visible;
            Invert = invert;
            KeepOriginalAlpha = keepOriginalAlpha;
            AlphaChannel = alphaChannel;
            Control = control;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> KeepOriginalAlpha { get; set; }
        public GenericValue<AlphaChannel> AlphaChannel { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<bool> Visible { get; set; }


        [ObservableProperty]
        private bool isExpanded;
    }
}
