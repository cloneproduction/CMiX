// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    public class LayerModel : IControlModel, IPrefabModel
    {
        public LayerModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;

            LayerSettings = new LayerSettingsModel();

            IsMask = new GenericValueModel<bool>(false);

            AmbientOcclusion = new AmbientOcclusionModel();
            LocalReflection = new LocalReflectionModel();

            MaskChannel = new GenericValueModel<MaskChannel>(Texturing.MaskChannel.Alpha);
            MaskMode = new GenericValueModel<MaskMode>(Texturing.MaskMode.AllBelow);
            Invert = new GenericValueModel<bool>(false);
            TextureModifierManager = new PrefabManagerModel();
            ModelEntityManager = new PrefabManagerModel();
            ModifierManager = new PrefabManagerModel();
        }


        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public LayerSettingsModel LayerSettings { get; set; }



        public GenericValueModel<bool> Invert { get; set; }
        public GenericValueModel<bool> IsMask { get; set; }
        public GenericValueModel<MaskMode> MaskMode { get; set; }

        public PrefabManagerModel TextureModifierManager { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }

        public AmbientOcclusionModel AmbientOcclusion { get; set; }
        public LocalReflectionModel LocalReflection { get; set; }

  
        public GenericValueModel<MaskChannel> MaskChannel { get; set; }
        public PrefabManagerModel ModelEntityManager { get; set; }

    }
}
