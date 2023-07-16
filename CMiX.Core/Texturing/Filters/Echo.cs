// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : ObservableObject, IModifier
    {
        public Echo()
        {
            ID = Guid.NewGuid();
            Visible = new BooleanValue();
            Factor = new FloatValue();
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public FloatValue Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded;

    }
}
