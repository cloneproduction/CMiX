// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                DiffuseTexture.CompositionID = value;
                MaskTexture.CompositionID = value;
            }
        }

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
