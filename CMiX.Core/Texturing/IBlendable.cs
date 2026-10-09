// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing
{
    // Something that is blended with a Blend Mode and an Opacity: the settings of a Layer, a Composition or a filter.
    public interface IBlendable
    {
        GenericValue<BlendMode> BlendMode { get; }
        GenericValue<float> Opacity { get; }
    }
}
