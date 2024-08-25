// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;

namespace CMiX.Core.Materials
{
    public class MaterialModel : IControlModel, IPrefabModel
    {
        public MaterialModel()
        {
            DiffuseTexture = new DiffuseTextureModel();
            MaskTexture = new MaskTextureModel();
            MaterialSettings = new MaterialSettingsModel();
            PrefabService = new PrefabServiceModel();
            ExplodeTriangleTextureManager = new PrefabManagerModel();
            ExplodeStrength = new GenericValueModel<float>(0.6f);
            ModifierManager = new PrefabManagerModel();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; }
        public MaskTextureModel MaskTexture { get; set; }
        public MaterialSettingsModel MaterialSettings { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel ExplodeTriangleTextureManager { get; set; }
        public GenericValueModel<float> ExplodeStrength { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
    }
}
