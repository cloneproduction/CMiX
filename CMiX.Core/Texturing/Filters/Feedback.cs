// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : ObservableObject, ITextureModifier
    {
        public Feedback(GenericValue<bool> visible, GenericValue<float> factor)
        {
            Visible = visible; // new GenericValue<bool>(true);
            Factor = factor; // new GenericValue<float>(0.9f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> Visible { get; set; }
        public GenericValue<float> Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
