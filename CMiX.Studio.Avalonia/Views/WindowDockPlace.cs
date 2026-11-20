// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views
{
    // The place of a docked window: the position in screen pixels, the size in device independent pixels.
    public readonly record struct WindowDockPlace(PixelPoint Position, double Width, double Height);
}
