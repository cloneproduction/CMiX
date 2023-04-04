// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.ViewModels.Components;

namespace CMiX.Core.Presentations.Components
{
    public class Layer : Component, IPrefab
    {
        public Layer(LayerModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            Name = new StringValue(layerModel.Name);
            IsRenaming = new BooleanValue(layerModel.IsRenaming);
            IsSelected = new BooleanValue(layerModel.IsSelected);

            CompositionService = compositionService;

            Visibility = new BooleanValue(layerModel.Visibility);
            IsMask = new BooleanValue(layerModel.IsMask);

            Opacity = new FloatValue(layerModel.Opacity);

            BackgroundColor = new ColorSelector(layerModel.BackgroundColor);

            MaskChannel = new GenericValue<MaskChannel>(layerModel.MaskChannelModel);
            BlendMode = new GenericValue<BlendModeEnum>(layerModel.BlendModeModel);
            MaskMode = new GenericValue<MaskMode>(layerModel.MaskModeModel);

            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);

            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService), compositionService);
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
    }
}
