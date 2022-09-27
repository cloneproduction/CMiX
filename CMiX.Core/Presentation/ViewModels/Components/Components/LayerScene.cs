// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class LayerScene : Component, ILayer, IRecipient<MessageKeyPressed>
    {
        public LayerScene(LayerSceneModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            CompositionService = compositionService;

            Opacity = new Slider(nameof(Opacity), layerModel.Opacity);

            BackgroundColor = new ColorSelector(layerModel.ColorSelectorModel);

            EditPanelOpen = new ToggleButton(layerModel.EditPanelOpen);
            Visibility = new ToggleButton(layerModel.VisibilityModel);
            BlendMode = new ComboBox<BlendModeEnum>(layerModel.BlendModeModel);

            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            SelectedTabIndex = 0;
            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService));

            ModelEntityManager = new PrefabManager<Entity>(layerModel.ModelEntityManager, compositionService.PrefabFactory);
            CameraEntityManager = new PrefabManager<Camera>(layerModel.CameraEntityManager, compositionService.PrefabFactory);
            LightEntityManager = new PrefabManager<LightEntity>(layerModel.LightEntityManager, compositionService.PrefabFactory);
        }


        public ICommand OpenColorSelectorCommand { get; set; }


        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public PrefabManager<Camera> CameraEntityManager { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }

        public void OpenColorSelector()
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }


        public CompositionService CompositionService { get; set; }
        public Slider Opacity { get; set; }
        public ToggleButton EditPanelOpen { get; set; }
        public ToggleButton Visibility { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ComboBox<BlendModeEnum> BlendMode { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }


        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }


        public override IModel GetModel()
        {
            LayerSceneModel model = new LayerSceneModel();

            model.ID = this.ID;
            model.Name = this.Name;

            model.TextureModifierManager = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.ColorSelectorModel = (ColorSelectorModel)this.BackgroundColor.GetModel();
            model.VisibilityModel = (ToggleButtonModel)this.Visibility.GetModel();
            model.AmbientOcclusion = (AmbientOcclusionModel)this.AmbientOcclusion.GetModel();
            model.BlendModeModel = (ComboBoxModel<BlendModeEnum>)this.BlendMode.GetModel();
            model.Opacity = (SliderModel)this.Opacity.GetModel();
            model.ModelEntityManager = (PrefabManagerModel)this.ModelEntityManager.GetModel();
            model.CameraEntityManager = (PrefabManagerModel)this.CameraEntityManager.GetModel();
            model.LightEntityManager = (PrefabManagerModel)(this.LightEntityManager.GetModel());

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerSceneModel layerModel = model as LayerSceneModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;

            this.TextureModifierManager.SetViewModel(layerModel.TextureModifierManager);
            this.BackgroundColor.SetViewModel(layerModel.ColorSelectorModel);
            this.Visibility.SetViewModel(layerModel.VisibilityModel);
            this.AmbientOcclusion.SetViewModel(layerModel.AmbientOcclusion);
            this.BlendMode.SetViewModel(layerModel.BlendModeModel);
            this.Opacity.SetViewModel(layerModel.Opacity);
            this.ModelEntityManager.SetViewModel(layerModel.ModelEntityManager);
            this.CameraEntityManager.SetViewModel(layerModel.Camera);
            this.LightEntityManager.SetViewModel(layerModel.LightEntityManager);
        }

        public void Receive(MessageKeyPressed message)
        {
            if (!IsSelected)
                return;

            switch (message.Key)
            {
                case Key.D1:
                    SelectedTabIndex = 0;
                    break;

                case Key.D2:
                    SelectedTabIndex = 1;
                    break;

                case Key.D3:
                    SelectedTabIndex = 2;
                    break;

                case Key.D4:
                    SelectedTabIndex = 3;
                    break;

                case Key.D5:
                    SelectedTabIndex = 4;
                    break;
            }
        }
    }
}
