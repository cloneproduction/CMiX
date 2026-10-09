// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Managers
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
