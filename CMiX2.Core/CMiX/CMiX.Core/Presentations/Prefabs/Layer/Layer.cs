// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Components
{
    public class Layer : ObservableObject, IPrefab
    {
        public Layer(LayerModel layerModel, CompositionService compositionService)
        {
            ID = layerModel.ID;
            Name = new StringValue(layerModel.Name, compositionService);
            IsRenaming = new BooleanValue(layerModel.IsRenaming, compositionService);
            IsSelected = new BooleanValue(layerModel.IsSelected, compositionService);
            CompositionService = compositionService;
            Visibility = new BooleanValue(layerModel.Visibility, compositionService);
            IsMask = new BooleanValue(layerModel.IsMask, compositionService);
            Opacity = new FloatValue(layerModel.Opacity, compositionService);
            BackgroundColor = new ColorSelector(layerModel.BackgroundColor, compositionService);
            MaskChannel = new GenericValue<MaskChannel>(layerModel.MaskChannelModel, compositionService);
            BlendMode = new GenericValue<BlendModeEnum>(layerModel.BlendModeModel, compositionService);
            MaskMode = new GenericValue<MaskMode>(layerModel.MaskModeModel, compositionService);
            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion, compositionService);
            TextureModifierManager = new ModifierManager(layerModel.TextureModifierManager, new TextureFilterFactory(compositionService), compositionService);
            ModelEntityManager = new PrefabManager<Entity>(layerModel.ModelEntityManager.ID, compositionService, compositionService.EntityRepository);
            CameraEntityManager = new PrefabManager<Camera>(layerModel.CameraEntityManager, compositionService);
            LightEntityManager = new PrefabManager<LightEntity>(layerModel.LightEntityManager, compositionService);
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
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
