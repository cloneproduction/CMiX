// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class FullscreenQuad : ObservableObject, ITexturable, IPrefab, IHasCompositionID, IDisposable
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

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                Texture.CompositionID = value;
            }
        }

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

        public void Dispose() => DisposeAll(Texture.DiffuseTexture.TextureManager, Texture.MaskTexture.TextureManager);
    }
}
