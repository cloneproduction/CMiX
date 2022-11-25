// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
    public class Layer : Component, IPrefab
    {
        public Layer(LayerModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            Name = layerModel.Name;
            CompositionService = compositionService;

            Visibility = new ToggleButton(layerModel.Visibility);
            IsMask = new ToggleButton(layerModel.IsMask);

            Opacity = new Slider(nameof(Opacity), layerModel.Opacity);

            BackgroundColor = new ColorSelector(layerModel.ColorSelectorModel);

            MaskChannel = new ComboBox<MaskChannel>(layerModel.MaskChannelModel);
            BlendMode = new ComboBox<BlendModeEnum>(layerModel.BlendModeModel);
            MaskMode = new ComboBox<MaskMode>(layerModel.MaskModeModel);


            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);
            OpenColorSelectorCommand = new RelayCommand(OpenColorSelector);

            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService));

            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService));
            ModelEntityManager = new PrefabManager<Entity>(layerModel.ModelEntityManager, compositionService.PrefabFactory);
            CameraEntityManager = new PrefabManager<Camera>(layerModel.CameraEntityManager, compositionService.PrefabFactory);
            LightEntityManager = new PrefabManager<LightEntity>(layerModel.LightEntityManager, compositionService.PrefabFactory);
        }


        public CompositionService CompositionService { get; set; }
        public ToggleButton Visibility { get; set; }
        public ToggleButton IsMask { get; set; }
        
        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public PrefabManager<Camera> CameraEntityManager { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }
        public ModifierManager TextureModifierManager { get; set; }

        public ComboBox<BlendModeEnum> BlendMode { get; set; }
        public ComboBox<MaskMode> MaskMode { get; set; }
        public ComboBox<MaskChannel> MaskChannel { get; set; }

        public AmbientOcclusion AmbientOcclusion { get; set; }


        public Slider Opacity { get; set; }
        public ColorSelector BackgroundColor { get; set; }

        public ICommand OpenColorSelectorCommand { get; set; }
        public void OpenColorSelector()
        {
            CompositionService.DialogService.Show<ColorSelectorWindow>(this, this.BackgroundColor);
        }


        private int _selectedTabItemIndex;
        public int SelectedTabItemIndex
        {
            get => _selectedTabItemIndex;
            set => SetProperty(ref _selectedTabItemIndex, value);
        }


        public override IModel GetModel()
        {
            LayerModel model = new LayerModel();

            model.ID = ID;
            model.Name = Name;
            model.Visibility = (ToggleButtonModel)Visibility.GetModel();
            model.IsMask = (ToggleButtonModel)IsMask.GetModel();
            model.TextureModifierManager = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.MaskChannelModel = (ComboBoxModel<MaskChannel>)this.MaskChannel.GetModel();
            model.MaskModeModel = (ComboBoxModel<MaskMode>)this.MaskMode.GetModel();
            model.BlendModeModel = (ComboBoxModel<BlendModeEnum>)this.BlendMode.GetModel();
            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerModel layerModel = model as LayerModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;
            this.Visibility.SetViewModel(layerModel.Visibility);
            this.IsMask.SetViewModel(layerModel.IsMask);
            this.TextureModifierManager.SetViewModel(layerModel.TextureModifierManager);
            this.MaskChannel.SetViewModel(layerModel.MaskChannelModel);
            this.MaskMode.SetViewModel(layerModel.MaskModeModel);
            this.BlendMode.SetViewModel(layerModel.BlendModeModel);
        }
    }
}
