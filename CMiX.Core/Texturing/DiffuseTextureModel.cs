// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing
{
    public record DiffuseTextureModel : IControlModel
    {
        public Guid ID { get ; init; } = Guid.NewGuid();
        public PrefabManagerModel TextureManager { get; init; } = new();
        public PrefabManagerModel TextureFilterManager { get; init; } = new();
        public Transform2DModel Transform2D { get; init; } = new();
        public SamplerStateModel SamplerState { get; init; } = new();
    }
}
