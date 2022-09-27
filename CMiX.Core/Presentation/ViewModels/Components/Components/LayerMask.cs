// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Presentation.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class LayerMask : Component, ILayer
    {
        public LayerMask(LayerMaskModel maskModel, CompositionService compositionService)
        {
            ID = maskModel.ID;
            CompositionService = compositionService;

            Opacity = new Slider(nameof(Opacity), maskModel.Opacity);

            BackgroundColor = new ColorSelector(maskModel.ColorSelectorModel);

            EditPanelOpen = new ToggleButton(maskModel.EditPanelOpen);
            Visibility = new ToggleButton(maskModel.VisibilityModel);
            Invert = new ToggleButton(maskModel.Invert);

            TextureModifierManager = new ModifierManager(maskModel.ModifierManager, new TextureFilterFactory(compositionService));

            AmbientOcclusion = new AmbientOcclusion(maskModel.AmbientOcclusion);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            EntityPanelIsSelected = true;
            MaskChannelSelector = new ComboBox<MaskChannel>(maskModel.MaskChannelSelector);

            ModelEntityManager = new PrefabManager<Entity>(maskModel.ModelEntityManager, compositionService.PrefabFactory);
            CameraEntityManager = new PrefabManager<Camera>(maskModel.CameraEntityManager, compositionService.PrefabFactory);
            LightEntityManager = new PrefabManager<LightEntity>(maskModel.LightEntityManager, compositionService.PrefabFactory);
        }


        public ICommand OpenColorSelectorCommand { get; set; }
        public ICommand CreateEntityCommand { get; set; }
        public ICommand RemoveSelectedEntityCommand { get; set; }


        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public PrefabManager<Camera> CameraEntityManager { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }

        public CompositionService CompositionService { get; set; }

        public void OpenColorSelector()
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }


        private ObservableCollection<LightEntity> _lightEntities;
        public ObservableCollection<LightEntity> LightEntities
        {
            get => _lightEntities;
            set => SetProperty(ref _lightEntities, value);
        }


        private bool _entityPanelIsSelected;
        public bool EntityPanelIsSelected
        {
            get => _entityPanelIsSelected;
            set => SetProperty(ref _entityPanelIsSelected, value);
        }

        public ToggleButton EditPanelOpen { get; set; }
        public ToggleButton Visibility { get; set; }
        public ColorSelector BackgroundColor { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public Slider Opacity { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public ComboBox<MaskChannel> MaskChannelSelector { get; set; }
        public ToggleButton Invert { get; set; }


        public override IModel GetModel()
        {
            LayerMaskModel model = new LayerMaskModel();

            model.ID = this.ID;
            model.Name = this.Name;

            model.Opacity = (SliderModel)this.Opacity.GetModel();
            model.ModifierManager = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.ColorSelectorModel = (ColorSelectorModel)this.BackgroundColor.GetModel();
            model.VisibilityModel = (ToggleButtonModel)this.Visibility.GetModel();
            model.AmbientOcclusion = (AmbientOcclusionModel)this.AmbientOcclusion.GetModel();
            model.Invert = (ToggleButtonModel)this.Invert.GetModel();
            model.ModelEntityManager = (PrefabManagerModel)ModelEntityManager.GetModel();
            model.CameraEntityManager = (PrefabManagerModel)CameraEntityManager.GetModel();
            model.LightEntityManager = (PrefabManagerModel)LightEntityManager.GetModel();

            model.MaskChannelSelector = (ComboBoxModel<MaskChannel>)this.MaskChannelSelector.GetModel();

            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerMaskModel layerModel = model as LayerMaskModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;

            this.Opacity.SetViewModel(layerModel.Opacity);
            this.TextureModifierManager.SetViewModel(layerModel.ModifierManager);
            this.BackgroundColor.SetViewModel(layerModel.ColorSelectorModel);
            this.Visibility.SetViewModel(layerModel.VisibilityModel);
            this.AmbientOcclusion.SetViewModel(layerModel.AmbientOcclusion);
            this.CameraEntityManager.SetViewModel(layerModel.CameraEntityManager);
            this.LightEntityManager.SetViewModel(layerModel.LightEntityManager);

            this.ModelEntityManager.SetViewModel(layerModel.ModelEntityManager);

            this.MaskChannelSelector.SetViewModel(layerModel.MaskChannelSelector);
            this.Invert.SetViewModel(layerModel.Invert);
        }
    }
}
