// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : ObservableObject, ITextureModifier
    {
        public Feedback(BooleanValue visible, FloatValue factor)
        {
            Visible = visible; // new BooleanValue(true);
            Factor = factor; // new FloatValue(0.9f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public FloatValue Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
