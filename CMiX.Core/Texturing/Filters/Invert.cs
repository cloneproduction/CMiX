// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Invert : ObservableObject, IPrefab
    {
        public Invert(PrefabService prefabService,
                      GenericValue<float> factor, 
                      GenericValue<bool> invertAlpha, 
                      GenericValue<InvertChannel> invertChannel, 
                      GenericValue<float> control)
        {
            PrefabService = prefabService;
            Factor = factor;
            InvertAlpha = invertAlpha;
            InvertChannelSelector = invertChannel;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Factor { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> InvertAlpha { get; set; }
        public GenericValue<InvertChannel> InvertChannelSelector { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
