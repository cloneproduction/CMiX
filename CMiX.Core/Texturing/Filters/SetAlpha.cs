// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class SetAlpha : ObservableObject, IControl, IPrefab, ITextureModifier
    {
        public SetAlpha(PrefabService prefabService,
                        GenericValue<bool> invert,
                        GenericValue<bool> keepOriginalAlpha,
                        GenericValue<AlphaChannel> alphaChannel, 
                        GenericValue<float> control)
        {
            PrefabService = prefabService;
            Invert = invert;
            KeepOriginalAlpha = keepOriginalAlpha;
            AlphaChannel = alphaChannel;
            Control = control;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> KeepOriginalAlpha { get; set; }
        public GenericValue<AlphaChannel> AlphaChannel { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
