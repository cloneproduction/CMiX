// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : ObservableObject, IModifier
    {
        public Blur()
        {
            isExpanded = true;
            Strength = new FloatValue(0.5f);
            Visible = new BooleanValue(true);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue Strength { get; set; }
        public BooleanValue Visible { get; set; }


        [ObservableProperty]
        private bool isExpanded;
    }
}
