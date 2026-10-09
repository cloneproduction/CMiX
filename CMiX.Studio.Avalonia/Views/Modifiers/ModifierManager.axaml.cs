// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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

        // Forwarded onto each item's ModifierPanel; see ModifierPanel.PanelBackground for why a
        // null value here is safe (it simply leaves the item's default theme color untouched).
        public static readonly StyledProperty<IBrush> PanelBackgroundProperty =
            AvaloniaProperty.Register<ModifierManager, IBrush>(nameof(PanelBackground));
        public IBrush PanelBackground
        {
            get => GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
        }
    }
}
