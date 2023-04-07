// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Invert : ObservableObject, ITextureFilter
    {
        public Invert(InvertModel invertModel, CompositionService compositionService)
        {
            ID = invertModel.ID;
            Name = invertModel.Name;

            Factor = new FloatValue(invertModel.Factor, compositionService);
            Visible = new BooleanValue(invertModel.Visible, compositionService);
            InvertAlpha = new BooleanValue(invertModel.InvertAlpha, compositionService);
            InvertChannelSelector = new GenericValue<InvertChannel>(invertModel.InvertChannelSelector, compositionService);
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
