// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IControl, IPrefab, ITextureModifiable
    {
        public Layer()
        {
            
        }
        public Layer(PrefabService prefabService,
                     LayerSettings layerSettings,
                     LayerMaskService layerMaskService,
                     PrefabManager reorderablePrefabManager,
                     PrefabManager textureModifierManager,
                     PrefabManager layerModifier)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;

            Opacity = layerSettings.Opacity;
            BackgroundColor = layerSettings.BackgroundColor;
            BlendMode = layerSettings.BlendMode;
            AmbientOcclusion = layerSettings.AmbientOcclusion;
            LocalReflection = layerSettings.LocalReflection;
            IsMask = layerMaskService.IsMask;
            MaskChannel = layerMaskService.MaskChannel;
            MaskMode = layerMaskService.MaskMode;
            Invert = layerMaskService.Invert;

            TextureModifierManager = textureModifierManager;
            ModelEntityManager = reorderablePrefabManager;
            LayerModifierManager = layerModifier;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }

        public PrefabManager ModelEntityManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public PrefabManager LayerModifierManager { get; set; }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public LocalReflection LocalReflection { get; set; }
        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }

        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}
