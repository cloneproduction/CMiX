// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.UI.Interaction
{
    // The value editors ask this host for a scope when a drag starts and dispose the handle when
    // it ends. The application sets BeginScopeHandler once, in App.Initialize. Without a handler
    // the editors work and open no scope.
    public static class InteractionHost
    {
        public static Func<IDisposable>? BeginScopeHandler { get; set; }

        public static IDisposable? BeginScope() => BeginScopeHandler?.Invoke();
    }
}
