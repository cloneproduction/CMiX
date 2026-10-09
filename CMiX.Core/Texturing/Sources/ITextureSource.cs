// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;

namespace CMiX.Core.Texturing.Sources
{
    public interface ITextureSource : ITextureModifiable
    {
        Integer2 Resolution { get; set; }
        GenericValue<bool> UseCompositionResolution { get; set; }
    }
}
