// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Models.Component
{
    public class LayerMaskModel : IComponentModel
    {
        public LayerMaskModel()
        {
            ID = Guid.NewGuid();
            BlendMode = new BlendModeModel();
            Opacity = new SliderModel();
            Opacity.Amount = 1.0f;
            VisibilityModel = new ToggleButtonModel(false);
            ModifierManager = new ModifierManagerModel();
            ColorSelectorModel = new ColorSelectorModel();
            Camera = new CameraModel();
            AmbientOcclusion = new AmbientOcclusionModel();
            Invert = new ToggleButtonModel(true);

            MaskChannelSelector = new ComboBoxModel<MaskChannel>(MaskChannel.Luma);

            ModelEntityManager = new PrefabManagerModel();
            CameraEntityManager = new PrefabManagerModel();
            LightEntityManager = new PrefabManagerModel();

            EditPanelOpen = new ToggleButtonModel(false);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsVisible { get; set; }


        public ToggleButtonModel VisibilityModel { get; set; }
        public BlendModeModel BlendMode { get; set; }
        public SliderModel Opacity { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public CameraModel Camera { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public ComboBoxModel<MaskChannel> MaskChannelSelector { get; set; }
        public ToggleButtonModel Invert { get; internal set; }
        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public ToggleButtonModel EditPanelOpen { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
    }
}
