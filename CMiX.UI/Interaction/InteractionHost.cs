// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.UI.Interaction
{
    // The value editors hand the value they drag to the application through this host. Without a handler nothing opens.
    public static class InteractionHost
    {
        public static Func<object?, IDisposable?>? BeginDragHandler { get; set; }

        public static IDisposable? BeginDrag(object? value) => BeginDragHandler?.Invoke(value);
    }
}
