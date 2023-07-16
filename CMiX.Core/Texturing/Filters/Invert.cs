// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, IModifier
    {
        public Invert(InvertModel invertModel)
        {
            ID = Guid.NewGuid();

            Factor = new FloatValue();
            Visible = new BooleanValue();
            InvertAlpha = new BooleanValue();
            InvertChannelSelector = new GenericValue<InvertChannel>();
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
