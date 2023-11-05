// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : ObservableObject, ITextureModifier
    {
        public Edge(BooleanValue visible, FloatValue radius, FloatValue brightness, FloatValue control)
        {
            Visible = visible;
            Radius = radius;
            Brightness = brightness;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue Radius { get; set; }
        public FloatValue Brightness { get; set; }
        public FloatValue Control { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
