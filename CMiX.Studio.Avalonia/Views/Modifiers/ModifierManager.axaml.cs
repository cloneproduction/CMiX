// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ModifierManager : UserControl
    {
        public ModifierManager()
        {
            InitializeComponent();
        }

        // The WPF original typed this as FrameworkElement; Control is the Avalonia equivalent.
        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<ModifierManager, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }
    }
}
