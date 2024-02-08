// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, ITextureModifier
    {
        public Invert(GenericValue<bool> visible, GenericValue<float> factor, GenericValue<bool> invertAlpha, GenericValue<InvertChannel> invertChannel, GenericValue<float> control)
        {
            Visible = visible;
            Factor = factor;
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Factor { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
