// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IPrefab
    {
        public Layer()
        {
            
        }
        public Layer(PrefabService prefabService,
                     LayerSettings layerService,
                     LayerMaskService layerMaskService,
                     ReorderablePrefabManager reorderablePrefabManager,
                     ReorderablePrefabManager modifierManager)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;
            Name = prefabService.Name;
            IsRenaming = prefabService.IsRenaming;
            IsSelected = prefabService.IsSelected;
            Visibility = prefabService.Visibility;

            Opacity = layerService.Opacity;
            BackgroundColor = layerService.BackgroundColor;
            BlendMode = layerService.BlendMode;
            AmbientOcclusion = layerService.AmbientOcclusion;

            IsMask = layerMaskService.IsMask;
            MaskChannel = layerMaskService.MaskChannel;
            MaskMode = layerMaskService.MaskMode;
            Invert = layerMaskService.Invert;

            ModifierManager = modifierManager;
            ModelEntityManager = reorderablePrefabManager;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public GenericValue<string> Name { get; set; }
        public GenericValue<bool> IsRenaming { get; set; }
        public GenericValue<bool> IsSelected { get; set; }
        public GenericValue<bool> Visibility { get; set; }

        public GenericValue<bool> Invert { get; set; }
        public GenericValue<bool> IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }

        public ReorderablePrefabManager ModelEntityManager { get; set; }
        public ReorderablePrefabManager ModifierManager { get; set; }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public GenericValue<float> Opacity { get; set; }
        public GenericValue<string> BackgroundColor { get; set; }

        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}
