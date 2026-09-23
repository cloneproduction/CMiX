// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
