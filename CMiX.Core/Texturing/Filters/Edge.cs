// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : ObservableObject, ITextureFilter
    {
        public Edge(EdgeModel edgeModel)
        {
            ID = edgeModel.ID;
            Name = edgeModel.Name;
            Visible = new BooleanValue(edgeModel.Visible);
            Radius = new FloatValue(edgeModel.Radius);
            Brightness = new FloatValue(edgeModel.Brightness);
            Control = new FloatValue(edgeModel.Control);
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
