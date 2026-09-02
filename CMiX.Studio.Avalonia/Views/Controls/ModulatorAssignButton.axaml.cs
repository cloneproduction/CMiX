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
        // Popup-ready flattening of ModulatorManager.ManagerData.Items: a single-output modulator
        // (every modulator today) contributes exactly one ModulatorOutputSelection row, unchanged
        // from today's one-modulator-one-row rendering; a multi-output modulator instead
        // contributes one non-clickable IModulator "label" row (so it can render the modulator's
        // own name, undecorated) followed by one ModulatorOutputSelection row per entry in
        // OutputNames. Rebuilt from scratch on every relevant change - the list is always small
        // (one entry per modulator instance, times its OutputNames), so a full rebuild is simpler
        // than diffing in place and cheap enough not to matter.
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

            var flattened = new List<object>();
            foreach (var item in items)
            {
                if (item is not IModulator modulator)
                    continue;

                if (modulator.OutputNames.Count > 1)
                {
                    flattened.Add(modulator);
                    foreach (var outputName in modulator.OutputNames)
                        flattened.Add(new ModulatorOutputSelection(modulator, outputName));
                }
                else
                {
                    flattened.Add(new ModulatorOutputSelection(modulator, modulator.OutputNames.FirstOrDefault()));
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
