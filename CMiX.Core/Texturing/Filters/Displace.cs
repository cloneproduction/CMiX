// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Displace : ObservableObject, IPrefab, ITextureFilter
    {
        public Displace(PrefabService prefabService, 
                        PrefabManager textureSelector,
                        Vector2 offset,
                        Vector2 offsetScale, 
                        GenericValue<float> control)
        {
            PrefabService = prefabService;
            Control = control;
            Offset = offset;
            OffsetScale = offsetScale;
            TextureSelector = textureSelector;
            Control = control;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManager TextureSelector { get; set; }
        public Vector2 Offset { get; set; }
        public Vector2 OffsetScale { get; set; }
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
