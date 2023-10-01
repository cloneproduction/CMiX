// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : ObservableObject, ITextureModifier
    {
        public Edge( )
        {
            Visible = new BooleanValue(true);
            Radius = new FloatValue(0.2f);
            Brightness = new FloatValue(0.2f);
            Control = new FloatValue(1.0f);
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
