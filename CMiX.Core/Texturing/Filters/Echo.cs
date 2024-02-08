// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : ObservableObject, ITextureModifier
    {
        public Echo(GenericValue<bool> visible, GenericValue<float> factor)
        {
            ID = Guid.NewGuid();
            Visible = visible;
            Factor = factor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<float> Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

    }
}
