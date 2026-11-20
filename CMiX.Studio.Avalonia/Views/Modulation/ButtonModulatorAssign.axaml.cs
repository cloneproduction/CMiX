// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using CMiX.Core.Modulation;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    public partial class ButtonModulatorAssign : UserControlModulatable
    {
        // One collection for the whole life of the menu: a new ItemsSource would not refresh the open menu.
        private readonly ObservableCollection<ModulatorOutputChoice> _choices = new();
        private INotifyCollectionChanged _observedItems;

        static ButtonModulatorAssign()
        {
            ModulatorManagerProperty.Changed.AddClassHandler<ButtonModulatorAssign>((c, e) => c.OnModulatorManagerChanged());
        }

        public ButtonModulatorAssign()
        {
            InitializeComponent();
            Flyout.ItemsSource = _choices;
            Flyout.Opening += (s, e) => RebuildChoices();
        }

        private MenuFlyout Flyout => (MenuFlyout)assignButton.Flyout!;

        private void OnModulatorManagerChanged()
        {
            WatchItems();
            RebuildChoices();
        }

        private void WatchItems()
        {
            UnwatchItems();
            _observedItems = ModulatorManager?.ManagerData?.Items;
            if (_observedItems != null)
                _observedItems.CollectionChanged += OnItemsCollectionChanged;
        }

        private void UnwatchItems()
        {
            if (_observedItems != null)
                _observedItems.CollectionChanged -= OnItemsCollectionChanged;
            _observedItems = null;
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RebuildChoices();

        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);
            RebuildChoices();
        }

        // The menu rows are built again each time the menu opens, so the assigned row is current.
        private void RebuildChoices()
        {
            _choices.Clear();

            var items = ModulatorManager?.ManagerData?.Items;
            if (items == null)
                return;

            foreach (var choice in ModulatorOutputChoice.ForBindable(items, DataContext as IModulatorBindable))
                _choices.Add(choice);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            OnModulatorManagerChanged();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            UnwatchItems();
        }
    }
}
