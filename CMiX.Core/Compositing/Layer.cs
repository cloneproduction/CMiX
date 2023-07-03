// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IPrefab, IModifiable
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

            MaskChannel = new GenericValue<MaskChannel>(layerModel.MaskChannel);
            BlendMode = new GenericValue<BlendModeEnum>(layerModel.BlendMode);
            MaskMode = new GenericValue<MaskMode>(layerModel.MaskMode);

            AmbientOcclusion = new AmbientOcclusion(layerModel.AmbientOcclusion);
            ModifierManager = new ModifierManager(layerModel.ModifierManager, new TextureFilterFactory());

            ModelEntityManager = new PrefabManager(layerModel.ModelEntityManager.ID, compositionService, compositionService.EntityRepository);
            CameraEntityManager = new PrefabManager(layerModel.CameraEntityManager.ID, compositionService, compositionService.CameraRepository);
            LightEntityManager = new PrefabManager(layerModel.LightEntityManager.ID, compositionService, compositionService.LightEntityRepository);
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue IsSelected { get; set; }
        public CompositionService CompositionService { get; set; }
        public BooleanValue Visibility { get; set; }
        public BooleanValue IsMask { get; set; }
        public PrefabManager ModelEntityManager { get; set; }
        public PrefabManager CameraEntityManager { get; set; }
        public PrefabManager LightEntityManager { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public GenericValue<BlendModeEnum> BlendMode { get; set; }
        public GenericValue<MaskMode> MaskMode { get; set; }
        public GenericValue<MaskChannel> MaskChannel { get; set; }
        public AmbientOcclusion AmbientOcclusion { get; set; }
        public FloatValue Opacity { get; set; }
        public ColorSelector BackgroundColor { get; set; }

        [ObservableProperty]
        private int selectedTabItemIndex;
    }
}
