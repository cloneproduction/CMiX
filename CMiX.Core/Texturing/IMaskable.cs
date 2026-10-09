// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    // Something that masks with a Mask Channel and an Invert flag: the mask settings of a Layer or a Composition, or the mask texture of a texture.
    public interface IMaskable
    {
        GenericValue<MaskChannel> MaskChannel { get; }
        GenericValue<bool> Invert { get; }
    }
}
