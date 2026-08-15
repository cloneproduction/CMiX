// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ButtonOpenSelectionPanel : UserControl
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static ButtonOpenSelectionPanel()
        {
            LogicalPanelContent.Track<ButtonOpenSelectionPanel>(SelectionPanelProperty);
        }

        public ButtonOpenSelectionPanel()
        {
            InitializeComponent();
        }

        // WPF FrameworkElement becomes the Avalonia Control base type.
        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<ButtonOpenSelectionPanel, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ButtonOpenSelectionPanel, string>(nameof(Caption), string.Empty);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
