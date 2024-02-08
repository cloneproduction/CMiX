// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Texturing;

namespace CMiX.Core.Materials
{
    public class MaterialModel : IControlModel
    {
        public MaterialModel()
        {
            DiffuseTexture = new DiffuseTextureModel();
            MaskTexture = new MaskTextureModel();
            MaterialSettings = new MaterialSettingsModel();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; }
        public MaskTextureModel MaskTexture { get; set; }
        public MaterialSettingsModel MaterialSettings { get; set; }
    }
}
