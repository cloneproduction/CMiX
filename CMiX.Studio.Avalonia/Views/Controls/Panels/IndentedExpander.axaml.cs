// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls.Panels
{
    public partial class IndentedExpander : UserControl
    {
        public IndentedExpander()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<int> IndentationProperty =
            AvaloniaProperty.Register<IndentedExpander, int>(nameof(Indentation), 0, defaultBindingMode: BindingMode.TwoWay);
        public int Indentation
        {
            get => GetValue(IndentationProperty);
            set => SetValue(IndentationProperty, value);
        }

        public static readonly StyledProperty<object> HeaderProperty =
            AvaloniaProperty.Register<IndentedExpander, object>(nameof(Header));
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<IndentedExpander, bool>(nameof(IsExpanded), false, defaultBindingMode: BindingMode.TwoWay);
        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
    }
}
