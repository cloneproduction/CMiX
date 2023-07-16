// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : ObservableObject, IModifier
    {
        public Pixelate()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValue();
            Control = new FloatValue();
            Factor = new Vector2();
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public Vector2 Factor { get; set; }
        public FloatValue Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
