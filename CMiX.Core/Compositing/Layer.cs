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
    public partial class Layer : ObservableObject, IPrefab, IModifiable
    {
        public Layer(PrefabService prefabService,
                     LayerService layerService,
                     LayerMaskService layerMaskService,
                     PrefabManagerSlot prefabManagerSlot,
                     ModifierManager modifierManager)
        {
            ID = prefabService.ID;
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
            ModelEntityManager = prefabManagerSlot;
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue Visibility { get; set; }

        public BooleanValue Invert { get; set; }
        public BooleanValue IsMask { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }

        public PrefabManagerSlot ModelEntityManager { get; set; }
        public ModifierManager ModifierManager { get; set; }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public FloatValue Opacity { get; set; }
        public ColorValue BackgroundColor { get; set; }

        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}
