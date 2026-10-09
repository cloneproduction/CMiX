// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Layer : ObservableObject, IControl, IPrefab, ITextureModifiable, IModifiable, IHasCompositionID, IDisposable, IComposable
    {
        public Layer(PrefabService prefabService,
                     CompositingSettings compositingSettings,
                     MaskSettings maskSettings,
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

            Compositing = compositingSettings;
            Mask = maskSettings;
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

        public CompositingSettings Compositing { get; set; }
        public MaskSettings Mask { get; set; }

        public AmbientOcclusion AmbientOcclusion { get; set; }
        public LocalReflection LocalReflection { get; set; }


        public PrefabManager ModelEntityManager { get; set; }
        public PrefabManager CameraManager { get; set; }
        public PrefabManager LightManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public PrefabManager ModifierManager { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                ModelEntityManager.CompositionID = value;
                CameraManager.CompositionID = value;
                LightManager.CompositionID = value;
                TextureModifierManager.CompositionID = value;
                ModifierManager.CompositionID = value;
            }
        }

        [ObservableProperty]
        private int selectedTabItemIndex;

        public IControlModel ToModel() => new LayerModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Compositing = (CompositingSettingsModel)Compositing.ToModel(),
            Mask = (MaskSettingsModel)Mask.ToModel(),
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
            Compositing.FromModel(m.Compositing);
            Mask.FromModel(m.Mask);
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
