// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class FullscreenQuad : ObservableObject, ITexturable, IPrefab, IHasCompositionID
    {
        public FullscreenQuad(PrefabService prefabService,
                              Color color,
                              Texture texture,
                              Transform2D transformTexture)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Color = color;
            Texture = texture;
            TransformTexture = transformTexture;
        }

        public PrefabService PrefabService { get; set; }
        public Color Color { get; set; }
        public Texture Texture { get; set; }
        public Transform2D TransformTexture { get; set; }

        public Guid ID { get; set; }
        public Guid CompositionID { get; set; }

        public IControlModel ToModel() => new FullscreenQuadModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Color = (ColorModel)Color.ToModel(),
            Texture = (TextureModel)Texture.ToModel(),
            TransformTexture = (Transform2DModel)TransformTexture.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (FullscreenQuadModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Color.FromModel(m.Color);
            Texture.FromModel(m.Texture);
            TransformTexture.FromModel(m.TransformTexture);
        }
    }
}
