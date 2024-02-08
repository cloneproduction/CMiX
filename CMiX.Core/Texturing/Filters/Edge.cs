// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : ObservableObject, ITextureModifier
    {
        public Edge(GenericValue<bool> visible, 
                    GenericValue<float> radius, 
                    GenericValue<float> brightness, 
                    GenericValue<float> control)
        {
            Visible = visible;
            Radius = radius;
            Brightness = brightness;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Brightness { get; set; }
        public GenericValue<float> Control { get; set; }
        public GenericValue<bool> Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
