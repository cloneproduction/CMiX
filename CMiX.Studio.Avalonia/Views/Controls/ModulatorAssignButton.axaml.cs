// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ModulatorAssignButton : ModulatorAssignableUserControl
    {
        // Popup-ready flattening of ModulatorManager.ManagerData.Items, filtered down to only the
        // outputs that actually fit the consuming field - one ModulatorOutputSelection row per
        // surviving output, each a plain clickable Button (see ModulatorAssignButton.axaml)
        // labeled "<Modulator> <Output>" (e.g. "Beat Value"). No group-label rows, no separators -
        // every row is self-contained and identical in shape, deliberately, so there's nothing here
        // to keep in sync between this method and how the rows render. A modulator with zero
        // surviving outputs (e.g. BeatModulator's float-only Value against an Integer-required
        // Count) contributes nothing. Rebuilt from scratch on every relevant change - the list is
        // always small (one entry per modulator instance, times its Outputs), so a full rebuild is
        // simpler than diffing in place and cheap enough not to matter.
        public static readonly StyledProperty<IEnumerable> FlattenedItemsProperty =
            AvaloniaProperty.Register<ModulatorAssignButton, IEnumerable>(nameof(FlattenedItems));
        public IEnumerable FlattenedItems
        {
            get => GetValue(FlattenedItemsProperty);
            private set => SetValue(FlattenedItemsProperty, value);
        }

        private INotifyCollectionChanged _observedItems;

        static ModulatorAssignButton()
        {
            ModulatorManagerProperty.Changed.AddClassHandler<ModulatorAssignButton>((c, e) => c.OnModulatorManagerChanged());
        }

        public ModulatorAssignButton()
        {
            InitializeComponent();
            OnModulatorManagerChanged();
        }

        // ModulatorManager.ManagerData.Items mutates in place (modulators get added/removed from
        // the modulator stack list) without ModulatorManager itself ever changing, so the flattened
        // list needs its own subscription to the collection's own change notifications, not just
        // to ModulatorManagerProperty - mirroring how ComboBox.axaml.cs tracks its bound
        // ItemsSource's CollectionChanged for the same reason.
        private void OnModulatorManagerChanged()
        {
            if (_observedItems != null)
                _observedItems.CollectionChanged -= OnItemsCollectionChanged;

            _observedItems = ModulatorManager?.ManagerData?.Items as INotifyCollectionChanged;
            if (_observedItems != null)
                _observedItems.CollectionChanged += OnItemsCollectionChanged;

            RebuildFlattenedItems();
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => RebuildFlattenedItems();

        // The filter below reads RequiredValueType off DataContext, but DataContext and
        // ModulatorManager are set through two independent bindings with no guaranteed order - if
        // ModulatorManager resolves first, the rebuild above runs with DataContext still unset,
        // filters nothing (see the fallback below), and then never reruns once DataContext actually
        // arrives, since nothing was listening for it. Confirmed live: this let every output through
        // for a field whose DataContext just hadn't settled yet when the collection-driven rebuild
        // fired. Overriding this ensures a rebuild always happens once DataContext is the real thing.
        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);
            RebuildFlattenedItems();
        }

        private void RebuildFlattenedItems()
        {
            var items = ModulatorManager?.ManagerData?.Items;
            if (items == null)
            {
                FlattenedItems = null;
                return;
            }

            // The consuming field's required numeric type - not exposed as its own StyledProperty,
            // just read straight off DataContext, same as AssignFromDataContext already does for the
            // click handler. Falls back to offering everything unfiltered if DataContext somehow
            // isn't an IModulatorBindable, rather than silently emptying the popup.
            var requiredValueType = (DataContext as IModulatorBindable)?.RequiredValueType;

            var flattened = new List<object>();
            foreach (var item in items)
            {
                if (item is not IModulator modulator)
                    continue;

                var outputs = requiredValueType is { } required
                    ? modulator.Outputs.Where(o => o.ValueType == required).ToList()
                    : modulator.Outputs.ToList();

                foreach (var output in outputs)
                    flattened.Add(new ModulatorOutputSelection(modulator, output));
            }

            FlattenedItems = flattened;
        }

        // Without this, ModulatorManager's own Items collection (owned by a project-lifetime
        // manager, not this popup) would keep a discarded control's handler alive for the rest of
        // the session - same leak ComboBox.axaml.cs guards against for the same reason.
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            if (_observedItems != null)
            {
                _observedItems.CollectionChanged -= OnItemsCollectionChanged;
                _observedItems = null;
            }
        }

        private void Assign(object sender, RoutedEventArgs e)
        {
            AssignFromDataContext(sender);
            assignButton.Flyout!.Hide();
        }
    }
}
