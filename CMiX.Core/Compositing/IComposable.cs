// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modifiers;

namespace CMiX.Core.Compositing
{
    // An item that is blended, masked and modified in a stack: a Layer or a Composition.
    public interface IComposable : IModifiable
    {
        CompositingSettings Compositing { get; }
        MaskSettings Mask { get; }
    }
}
