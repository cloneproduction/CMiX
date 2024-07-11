// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Feedback : ObservableObject, IPrefab, ITextureModifier
    {
        public Feedback(PrefabService prefabService,
                        GenericValue<bool> visible, 
                        GenericValue<float> factor)
        {
            PrefabService = prefabService;
            Factor = factor;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Factor { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
