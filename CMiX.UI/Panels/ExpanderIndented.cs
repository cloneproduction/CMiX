// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.UI.Panels
{
    public class ExpanderIndented : Expander
    {
        public static readonly StyledProperty<int> IndentationProperty =
            AvaloniaProperty.Register<ExpanderIndented, int>(nameof(Indentation), 0, defaultBindingMode: BindingMode.TwoWay);
        public int Indentation
        {
            get => GetValue(IndentationProperty);
            set => SetValue(IndentationProperty, value);
        }
    }
}
