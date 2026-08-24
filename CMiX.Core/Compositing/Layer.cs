// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IControl, IPrefab, ITextureModifiable, IModifiable, IDisposable
    {
        public Layer(PrefabService prefabService,
                     LayerSettings layerSettings,
                     LayerMaskSettings layerMaskSettings,
                     AmbientOcclusion ambientOcclusion,
                     LocalReflection localReflectionModel,
                     PrefabManager modelEntityManager,
                     PrefabManager cameraManager,
                     PrefabManager lightManager,
                     PrefabManager textureModifierManager,
                     PrefabManager layerModifier)
        {
            ID = prefabService.ID;

            PrefabService = prefabService;

            LayerSettings = layerSettings;
            LayerMaskSettings = layerMaskSettings;
            AmbientOcclusion = ambientOcclusion;
            LocalReflection = localReflectionModel;

            TextureModifierManager = textureModifierManager;
            ModelEntityManager = modelEntityManager;
            CameraManager = cameraManager;
            LightManager = lightManager;
            ModifierManager = layerModifier;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public LayerSettings LayerSettings { get; set; }
        public LayerMaskSettings LayerMaskSettings { get; set; }

        public AmbientOcclusion AmbientOcclusion { get; set; }
        public LocalReflection LocalReflection { get; set; }


        public PrefabManager ModelEntityManager { get; set; }
        public PrefabManager CameraManager { get; set; }
        public PrefabManager LightManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public PrefabManager ModifierManager { get; set; }


        [ObservableProperty]
        private int selectedTabItemIndex;

        public IControlModel ToModel() => new LayerModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            LayerSettings = (LayerSettingsModel)LayerSettings.ToModel(),
            LayerMaskSettings = (LayerMaskSettingsModel)LayerMaskSettings.ToModel(),
            AmbientOcclusion = (AmbientOcclusionModel)AmbientOcclusion.ToModel(),
            LocalReflection = (LocalReflectionModel)LocalReflection.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel(),
            ModelEntityManager = (PrefabManagerModel)ModelEntityManager.ToModel(),
            CameraManager = (PrefabManagerModel)CameraManager.ToModel(),
            LightManager = (PrefabManagerModel)LightManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (LayerModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            LayerSettings.FromModel(m.LayerSettings);
            LayerMaskSettings.FromModel(m.LayerMaskSettings);
            AmbientOcclusion.FromModel(m.AmbientOcclusion);
            LocalReflection.FromModel(m.LocalReflection);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
            LoadManager(ModifierManager, m.ModifierManager);
            LoadManager(ModelEntityManager, m.ModelEntityManager);
            LoadManager(CameraManager, m.CameraManager);
            LoadManager(LightManager, m.LightManager);
        }

        public void Dispose() => DisposeAll(ModelEntityManager, CameraManager, LightManager, TextureModifierManager, ModifierManager);
    }
}
