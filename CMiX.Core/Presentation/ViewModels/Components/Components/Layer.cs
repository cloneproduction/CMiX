// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.ViewModels.Services;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Layer : Component, IBeatable
    {
        public Layer(LayerModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            Name = layerModel.Name;

            CompositionService = compositionService;

            Opacity = new Slider(nameof(Opacity), layerModel.Opacity);
            BlendMode = new BlendMode(layerModel.BlendMode);
            Visibility = new ToggleButton(layerModel.Visibility);

            LayerScene = new LayerScene(layerModel.LayerScene, compositionService);

            EntityManager = new PrefabManager<Entity>(layerModel.EntityManager);

            TextureFilterModifierManager = new ModifierManager(layerModel.ModifierManager, new TextureFilterFactory());

            SelectedIndex = 1;
        }


        public Slider Opacity { get; set; }
        public BlendMode BlendMode { get; set; }
        public ToggleButton Visibility { get; set; }
        public ModifierManager TextureFilterModifierManager { get; set; }
        public CompositionService CompositionService { get; set; }
        public PrefabManager<Entity> EntityManager { get; set; }


        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        private LayerScene _layerScene;
        public LayerScene LayerScene
        {
            get => _layerScene;
            set => SetProperty(ref _layerScene, value);
        }

        private LayerMask _layerMask;
        public LayerMask LayerMask
        {
            get => _layerMask;
            set => SetProperty(ref _layerMask, value);
        }


        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
            this.LayerScene.MasterBeat = masterBeat;
        }


        public override IComponentModel GetModel()
        {
            LayerModel model = new LayerModel();

            model.ID = this.ID;
            model.Name = this.Name;
            model.Opacity = (SliderModel)Opacity.GetModel();
            model.BlendMode =(BlendModeModel)BlendMode.GetModel();
            model.Visibility = (ToggleButtonModel)Visibility.GetModel();

            model.ModifierManager = (ModifierManagerModel)this.TextureFilterModifierManager.GetModel();

            model.LayerScene = (LayerSceneModel)LayerScene.GetModel();
            model.LayerMask = (LayerMaskModel)this.LayerMask?.GetModel();

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            LayerModel layerModel = model as LayerModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;
            this.Opacity.SetViewModel(layerModel.Opacity);
            this.BlendMode.SetViewModel(layerModel.BlendMode);
            this.Visibility.SetViewModel(layerModel.Visibility);

            this.TextureFilterModifierManager.SetViewModel(layerModel.ModifierManager);

            this.LayerScene.SetViewModel(layerModel.LayerScene);
            this.LayerMask?.SetViewModel(layerModel.LayerMask);
        }
    }
}
