// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views
{
    public partial class ModifierManager : UserControl
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static ModifierManager()
        {
            LogicalPanelContent.Track<ModifierManager>(SelectionPanelProperty);
        }

        public ModifierManager()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<ModifierManager, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ModifierManager, string>(nameof(Caption));
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
