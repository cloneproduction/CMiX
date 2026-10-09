// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public class IndentedExpander : Expander
    {
        public static readonly StyledProperty<int> IndentationProperty =
            AvaloniaProperty.Register<IndentedExpander, int>(nameof(Indentation), 0, defaultBindingMode: BindingMode.TwoWay);
        public int Indentation
        {
            get => GetValue(IndentationProperty);
            set => SetValue(IndentationProperty, value);
        }
    }
}
