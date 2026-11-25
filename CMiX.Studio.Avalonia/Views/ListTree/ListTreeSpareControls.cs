// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    // The controls that show nothing now. They stay hidden in the panel until a row or a card needs one.
    internal sealed class ListTreeSpareControls<T> where T : Control
    {
        private readonly Stack<T> controls = new();

        public void Keep(T control)
        {
            control.IsVisible = false;
            control.DataContext = null;
            controls.Push(control);
        }

        public T? Take()
        {
            if (!controls.TryPop(out var control))
                return null;

            control.IsVisible = true;
            return control;
        }
    }
}
