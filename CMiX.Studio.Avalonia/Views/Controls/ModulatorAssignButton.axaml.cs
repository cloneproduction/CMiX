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
