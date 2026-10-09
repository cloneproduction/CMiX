// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;

namespace CMiX.Core.Compositing
{
    public record FullscreenQuadModel : IPrefabModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabServiceModel PrefabService { get; set; } = new();
        public ColorModel Color { get; set; } = new();
        public TextureModel Texture { get; set; } = new();
        public Transform2DModel TransformTexture { get; set; } = new();
    }
}
