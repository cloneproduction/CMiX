// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatorAssignButton : ModulatorAssignableUserControl
    {
        // One collection for the whole life of the menu: a new ItemsSource would not refresh the open menu.
        private readonly ObservableCollection<ModulatorOutputChoice> _choices = new();
        private INotifyCollectionChanged _observedItems;

        static ModulatorAssignButton()
        {
            ModulatorManagerProperty.Changed.AddClassHandler<ModulatorAssignButton>((c, e) => c.OnModulatorManagerChanged());
        }

        public ModulatorAssignButton()
        {
            InitializeComponent();
            Flyout.ItemsSource = _choices;
            Flyout.Opening += (s, e) => RebuildChoices();
            OnModulatorManagerChanged();
        }

        private MenuFlyout Flyout => (MenuFlyout)assignButton.Flyout!;

        private void OnModulatorManagerChanged()
        {
            if (_observedItems != null)
                _observedItems.CollectionChanged -= OnItemsCollectionChanged;

            _observedItems = ModulatorManager?.ManagerData?.Items as INotifyCollectionChanged;
            if (_observedItems != null)
                _observedItems.CollectionChanged += OnItemsCollectionChanged;

            RebuildChoices();
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

            var bindable = DataContext as IModulatorBindable;

            // No bindable context means show every output.
            var selections = items.OfType<IModulator>().SelectMany(modulator => modulator.Outputs
                .Where(output => bindable?.CanBind(output) ?? true)
                .Select(output => new ModulatorOutputSelection(modulator, output)));

            foreach (var choice in ModulatorOutputChoice.Build(selections, bindable))
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
            if (_observedItems != null)
            {
                _observedItems.CollectionChanged -= OnItemsCollectionChanged;
                _observedItems = null;
            }
        }
    }
}
