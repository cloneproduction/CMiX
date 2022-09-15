// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Models
{
    public class LayerSceneModel : IComponentModel
    {
        public LayerSceneModel()
        {
            ID = Guid.NewGuid();

            EditPanelOpen = new ToggleButtonModel(true);
            VisibilityModel = new ToggleButtonModel();
            ComponentModels = new ObservableCollection<IComponentModel>();
            TextureModifierManager = new ModifierManagerModel();
            ColorSelectorModel = new ColorSelectorModel("#00000000");
            Camera = new CameraModel();
            AmbientOcclusion = new AmbientOcclusionModel();
            BlendModeModel = new BlendModeModel(BlendModeEnum.Normal);
            Opacity = new SliderModel(1.0f);
            ModelEntityManager = new PrefabManagerModel();
            CameraEntityManager = new PrefabManagerModel();
        }


        public Guid ID { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsVisible { get; set; }


        public ToggleButtonModel VisibilityModel { get; set; }
        public ObservableCollection<IComponentModel> ComponentModels { get; set; }
        public ModifierManagerModel TextureModifierManager { get; set; }
        public ColorSelectorModel ColorSelectorModel { get; set; }
        public CameraModel Camera { get; set; }
        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public BlendModeModel BlendModeModel { get; internal set; }
        public SliderModel Opacity { get; internal set; }
        public PrefabManagerModel ModelEntityManager { get; internal set; }
        public PrefabManagerModel CameraEntityManager { get; internal set; }
        public ToggleButtonModel EditPanelOpen { get; internal set; }
    }
}
