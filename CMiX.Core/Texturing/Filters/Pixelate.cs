// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : ObservableObject, ITextureModifier
    {
        public Pixelate()
        {
            Visible = new BooleanValue(true);
            Control = new FloatValue(1.0f);
            Factor = new Vector2(0.2f, 0.2f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
