// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IControl, IPrefab, ITextureModifiable, IModifiable
    {
        public Layer(PrefabService prefabService,
                     LayerSettings layerSettings,
                     LayerMaskSettings layerMaskService,
                     AmbientOcclusion ambientOcclusion,
                     LocalReflection localReflectionModel,
                     PrefabManager reorderablePrefabManager,
                     PrefabManager textureModifierManager,
                     PrefabManager layerModifier)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;

            LayerSettings = layerSettings;
            AmbientOcclusion = ambientOcclusion;
            LocalReflection = localReflectionModel;

            IsMask = layerMaskService.IsMask;
            MaskChannel = layerMaskService.MaskChannel;
            MaskMode = layerMaskService.MaskMode;
            Invert = layerMaskService.Invert;

            TextureModifierManager = textureModifierManager;
            ModelEntityManager = reorderablePrefabManager;
            ModifierManager = layerModifier;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public LayerSettings LayerSettings { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public LocalReflection LocalReflection { get; set; }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }

        public PrefabManager ModelEntityManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public PrefabManager ModifierManager { get; set; }


        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}
