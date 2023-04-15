// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, ITextureFilter
    {
        public Invert(InvertModel invertModel)
        {
            ID = invertModel.ID;
            Name = invertModel.Name;

            Factor = new FloatValue(invertModel.Factor);
            Visible = new BooleanValue(invertModel.Visible);
            InvertAlpha = new BooleanValue(invertModel.InvertAlpha);
            InvertChannelSelector = new GenericValue<InvertChannel>(invertModel.InvertChannelSelector);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }
        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
