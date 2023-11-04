// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : ObservableObject, ITextureModifier
    {
        public Echo(BooleanValue visible, FloatValue factor)
        {
            ID = Guid.NewGuid();
            Visible = visible;
            Factor = factor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public FloatValue Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

    }
}
