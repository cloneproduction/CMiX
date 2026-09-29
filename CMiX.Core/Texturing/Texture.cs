// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, IControl, IHasCompositionID
    {
        public Texture(DiffuseTexture diffuseTexture, 
                       MaskTexture maskTexture)
        {
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;
        }

        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public Guid ID { get; set; }
        public Guid CompositionID { get; set; }

        public IControlModel ToModel() => new TextureModel
        {
            ID = ID,
            DiffuseTexture = (DiffuseTextureModel)DiffuseTexture.ToModel(),
            MaskTexture = (MaskTextureModel)MaskTexture.ToModel(),
        };

        public void FromModel(IControlModel model)
        {
            var m = (TextureModel)model;
            ID = m.ID;
            DiffuseTexture.FromModel(m.DiffuseTexture);
            MaskTexture.FromModel(m.MaskTexture);
        }
    }
}
