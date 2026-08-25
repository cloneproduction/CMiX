// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class Outliner : UserControl
    {
        // Roots panel content in the logical tree so its deferred bindings apply; see LogicalPanelContent.
        static Outliner()
        {
            LogicalPanelContent.Track<Outliner>(SelectionPanelProperty);
        }

        public Outliner()
        {
            InitializeComponent();
        }

        // The ComboBox's SelectedItem binds one-way rather than two-way on purpose: assigning
        // SelectedItem on the PrefabSelector can synchronously remove the previously selected
        // item from ControlRepository's backing collection once it becomes unreferenced - the
        // same collection this ComboBox's ItemsSource displays. Doing that while Avalonia's
        // ComboBox is still mid-update for the selection change that triggered it throws
        // "Source collection was modified during selection update." Posting the assignment
        // defers it until after Avalonia's own update finishes.
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not PrefabManagerBase selector) return;
            var newItem = (sender as ComboBox)?.SelectedItem as IControl;
            Dispatcher.UIThread.Post(() => selector.SelectedItem = newItem);
        }

        // WPF FrameworkElement becomes the Avalonia Control base type.
        public static readonly StyledProperty<Control> SelectionPanelProperty =
            AvaloniaProperty.Register<Outliner, Control>(nameof(SelectionPanel));
        public Control SelectionPanel
        {
            get => GetValue(SelectionPanelProperty);
            set => SetValue(SelectionPanelProperty, value);
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<Outliner, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
