// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Edge : ObservableObject, ITextureFilter
    {

        public Edge(EdgeModel edgeModel, CompositionService compositionService)
        {
            ID = edgeModel.ID;
            Name = edgeModel.Name;
            Visible = new BooleanValue(edgeModel.Visible, compositionService);
            Radius = new FloatValue(edgeModel.Radius, compositionService);
            Brightness = new FloatValue(edgeModel.Brightness, compositionService);
            Control = new FloatValue(edgeModel.Control, compositionService);
            isExpanded = true;
        }

        public TextureFilterName Name { get; set; }
        public Guid ID { get; set; }
        public FloatValue Radius { get; set; }
        public FloatValue Brightness { get; set; }
        public FloatValue Control { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
