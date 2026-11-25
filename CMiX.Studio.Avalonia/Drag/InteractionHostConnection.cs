// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CMiX.Core;
using CMiX.Core.BaseControls;
using CMiX.UI.Interaction;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Studio.Avalonia.Drag
{
    // Connects the value editors of CMiX.UI to the one ValueInteraction of the container.
    // The editor hands its value. A value opens its own drag. Anything else opens the global scope.
    // A static with one side effect: it sets the one static of CMiX.UI, which the App rule on statics allows.
    public static class InteractionHostConnection
    {
        public static void Connect(IServiceProvider provider)
        {
            var interaction = provider.GetRequiredService<ValueInteraction>();
            InteractionHost.BeginScopeHandler = value =>
                value is IDraggable draggable ? draggable.BeginDrag() : interaction.BeginScope();
        }
    }
}
