// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, ITextureModifier
    {
        public Invert(BooleanValue visible, FloatValue factor, BooleanValue invertAlpha, GenericValue<InvertChannel> invertChannel)
        {
            Visible = visible;
            Factor = factor;
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue Factor { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
