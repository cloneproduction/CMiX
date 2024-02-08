// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : ObservableObject, ITextureModifier
    {
        public Pixelate(GenericValue<bool> visible, GenericValue<float> control, Vector2 factor)
        {
            Visible = visible; // new GenericValue<bool>(true);
            Control = control; // new GenericValue<float>(1.0f);
            Factor = factor; // new Vector2(0.2f, 0.2f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public Vector2 Factor { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
