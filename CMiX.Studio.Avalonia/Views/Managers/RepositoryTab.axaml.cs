// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Managers
{
    // Replaces the repeated MainWindow.axaml TabItem shape wrapping a RepositoryManager.
    public partial class RepositoryTab : TabItem
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static RepositoryTab()
        {
            LogicalPanelContent.Track<RepositoryTab>(SelectionPanelProperty);
            LogicalPanelContent.Track<RepositoryTab>(EditingPanelProperty);
        }

        public RepositoryTab()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<RepositoryTab, string>(nameof(Icon));
        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<string> ToolTipTextProperty =
            AvaloniaProperty.Register<RepositoryTab, string>(nameof(ToolTipText));
        public string ToolTipText
        {
            get => GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<RepositoryTab, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<RepositoryTab, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<Control> EditingPanelProperty =
            AvaloniaProperty.Register<RepositoryTab, Control>(nameof(EditingPanel));
        public Control EditingPanel
        {
            get => GetValue(EditingPanelProperty);
            set => SetValue(EditingPanelProperty, value);
        }
    }
}
