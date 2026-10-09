// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Texturing;

namespace CMiX.Core.Compositing
{
    // An item that is blended and masked in a stack: a Layer or a Composition.
    public interface IComposable
    {
        LayerSettings Compositing { get; }
        LayerMaskSettings Mask { get; }
    }
}
