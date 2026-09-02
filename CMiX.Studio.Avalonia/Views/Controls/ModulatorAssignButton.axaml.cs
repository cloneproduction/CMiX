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
        // outputs that actually fit the consuming field: a modulator with exactly one surviving
        // output (whether it only has one to begin with, or several but just one matches) contributes
        // a single ModulatorOutputSelection row, unchanged from today's one-modulator-one-row
        // rendering; a modulator with more than one surviving output instead contributes one
        // non-clickable IModulator "label" row (so it can render the modulator's own name,
        // undecorated) followed by one ModulatorOutputSelection row per surviving entry. A modulator
        // with zero surviving outputs (e.g. BeatModulator's float-only Value against an
        // Integer-required Count) is skipped entirely - no orphaned label with nothing under it.
        // Rebuilt from scratch on every relevant change - the list is always small (one entry per
        // modulator instance, times its Outputs), so a full rebuild is simpler than diffing in place
        // and cheap enough not to matter.
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

                if (outputs.Count == 0)
                    continue;

                // The group-label decision below must match what
                // ModulatorOutputSelectionToLabelConverter/ToIndentConverter each independently
                // recompute from selection.Modulator.Outputs.Count (the modulator's TOTAL output
                // count, unfiltered - they have no visibility into this method's RequiredValueType
                // filtering). Using the filtered outputs.Count here instead would desync the two:
                // a modulator with 2 total outputs but only 1 surviving filtering would render as an
                // unlabeled single row structurally, yet the converters would still see
                // Outputs.Count == 2 and indent/rename it as if it were grouped - an orphaned
                // indented row with no label above it. Always grouping whenever the modulator has
                // >1 output in total (even if only one currently fits this field) keeps both sides
                // of that decision using the same number.
                if (modulator.Outputs.Count > 1)
                {
                    flattened.Add(modulator);
                    foreach (var output in outputs)
                        flattened.Add(new ModulatorOutputSelection(modulator, output));
                }
                else
                {
                    flattened.Add(new ModulatorOutputSelection(modulator, outputs[0]));
                }
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
