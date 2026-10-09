// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;

namespace CMiX.Core.Texturing
{
    public interface ITexture
    {
        PrefabManager TextureManager { get; set; }
        Transform2D Transform2D { get; set; }
        SamplerState SamplerState { get; set; }
    }
}
