// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Layer : Component, IPrefab
    {
        public Layer(LayerModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            Name = layerModel.Name;
            SelectedTabItemIndex = 1;
            Visibility = new ToggleButton(layerModel.Visibility);

            LayerScene = new LayerScene(layerModel.LayerScene, compositionService);
            LayerMask = new LayerMask(layerModel.LayerMask, compositionService);

            TextureFilterModifierManager = new ModifierManager(layerModel.ModifierManager, new TextureFilterFactory(compositionService));
        }


        public ToggleButton Visibility { get; set; }
        public ModifierManager TextureFilterModifierManager { get; set; }


        private int _selectedTabItemIndex;
        public int SelectedTabItemIndex
        {
            get => _selectedTabItemIndex;
            set => SetProperty(ref _selectedTabItemIndex, value);
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


        public override IModel GetModel()
        {
            LayerModel model = new LayerModel();

            model.ID = ID;
            model.Name = Name;
            model.Visibility = (ToggleButtonModel)Visibility.GetModel();

            model.ModifierManager = (ModifierManagerModel)this.TextureFilterModifierManager.GetModel();
            model.LayerScene = (LayerSceneModel)LayerScene.GetModel();
            model.LayerMask = (LayerMaskModel)LayerMask?.GetModel();

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerModel layerModel = model as LayerModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;
            this.Visibility.SetViewModel(layerModel.Visibility);

            this.TextureFilterModifierManager.SetViewModel(layerModel.ModifierManager);
            this.LayerScene.SetViewModel(layerModel.LayerScene);
            this.LayerMask.SetViewModel(layerModel.LayerMask);
        }
    }
}
