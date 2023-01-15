// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
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

            Visibility = new BooleanValue(layerModel.Visibility);
            IsMask = new BooleanValue(layerModel.IsMask);

            Opacity = new FloatValue(layerModel.Opacity);

            BackgroundColor = new ColorSelector(layerModel.BackgroundColor);

            MaskChannel = new GenericValue<MaskChannel>(layerModel.MaskChannelModel);
            BlendMode = new GenericValue<BlendModeEnum>(layerModel.BlendModeModel);
            MaskMode = new GenericValue<MaskMode>(layerModel.MaskModeModel);

            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);

            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService));
            ModelEntityManager = new PrefabManager<Entity>(layerModel.ModelEntityManager.ID, compositionService, compositionService.EntityRepository);
            CameraEntityManager = new PrefabManager<Camera>(layerModel.CameraEntityManager, compositionService);
            LightEntityManager = new PrefabManager<LightEntity>(layerModel.LightEntityManager, compositionService);
        }


        public CompositionService CompositionService { get; set; }
        public BooleanValue Visibility { get; set; }
        public BooleanValue IsMask { get; set; }
        
        public PrefabManager<Entity> ModelEntityManager { get; set; }
        public PrefabManager<Camera> CameraEntityManager { get; set; }
        public PrefabManager<LightEntity> LightEntityManager { get; set; }
        public ModifierManager TextureModifierManager { get; set; }

        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }

        public AmbientOcclusion AmbientOcclusion { get; set; }


        public FloatValue Opacity { get; set; }
        public ColorSelector BackgroundColor { get; set; }



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
            model.Opacity = (FloatValueModel)Opacity.GetModel();

            model.ModelEntityManager = (PrefabManagerModel)ModelEntityManager.GetModel();
            model.TextureModifierManager = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.CameraEntityManager = (PrefabManagerModel)CameraEntityManager.GetModel();
            model.LightEntityManager = (PrefabManagerModel)LightEntityManager.GetModel();

            model.BackgroundColor = (ColorSelectorModel)BackgroundColor.GetModel();
            model.Visibility = (BooleanValueModel)Visibility.GetModel();
            model.IsMask = (BooleanValueModel)IsMask.GetModel();

            model.MaskChannelModel = (GenericValueModel<MaskChannel>)this.MaskChannel.GetModel();
            model.MaskModeModel = (GenericValueModel<MaskMode>)this.MaskMode.GetModel();
            model.BlendModeModel = (GenericValueModel<BlendModeEnum>)this.BlendMode.GetModel();
            return model;
        }

        public override void SetViewModel(IModel model)
        {
            LayerModel layerModel = model as LayerModel;

            this.ID = layerModel.ID;
            this.Name = layerModel.Name;
            this.Opacity.SetViewModel(layerModel.Opacity);

            this.ModelEntityManager.SetViewModel(layerModel.ModelEntityManager);
            this.TextureModifierManager.SetViewModel(layerModel.TextureModifierManager);
            this.CameraEntityManager.SetViewModel(layerModel.CameraEntityManager);
            this.LightEntityManager.SetViewModel(layerModel.LightEntityManager);

            this.BackgroundColor.SetViewModel(layerModel.BackgroundColor);
            this.Visibility.SetViewModel(layerModel.Visibility);
            this.IsMask.SetViewModel(layerModel.IsMask);

            this.MaskChannel.SetViewModel(layerModel.MaskChannelModel);
            this.MaskMode.SetViewModel(layerModel.MaskModeModel);
            this.BlendMode.SetViewModel(layerModel.BlendModeModel);
        }
    }
}
