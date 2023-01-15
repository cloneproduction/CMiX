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
            Opacity = new FloatValueModel();
            Opacity.Value = 1.0f;
            VisibilityModel = new BooleanValueModel(false);
            ModifierManager = new ModifierManagerModel();
            ColorSelectorModel = new ColorSelectorModel();
            Camera = new CameraModel();
            AmbientOcclusion = new AmbientOcclusionModel();
            Invert = new BooleanValueModel(true);

            MaskChannelSelector = new GenericValueModel<MaskChannel>(MaskChannel.Luma);

            ModelEntityManager = new PrefabManagerModel();
            CameraEntityManager = new PrefabManagerModel();
            LightEntityManager = new PrefabManagerModel();

            EditPanelOpen = new BooleanValueModel(false);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsVisible { get; set; }


        public BooleanValueModel VisibilityModel { get; set; }
        public BlendModeModel BlendMode { get; set; }
        public FloatValueModel Opacity { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public CameraModel Camera { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public GenericValueModel<MaskChannel> MaskChannelSelector { get; set; }
        public BooleanValueModel Invert { get; internal set; }
        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public BooleanValueModel EditPanelOpen { get; internal set; }
        public PrefabManagerModel LightEntityManager { get; internal set; }
    }
}
