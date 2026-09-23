// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaComboBox = Avalonia.Controls.ComboBox;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class ComboBox : CaptionedUserControl
    {
        private AvaloniaComboBox? _innerComboBox;

        public ComboBox()
        {
            InitializeComponent();
            _innerComboBox = this.FindControl<AvaloniaComboBox>("innerComboBox");

            _innerComboBox?.AddHandler(PointerWheelChangedEvent, OnInnerPointerWheelChanged, RoutingStrategies.Tunnel);
            _innerComboBox?.AddHandler(PointerPressedEvent, OnInnerPointerPressed, RoutingStrategies.Tunnel);

            if (_innerComboBox != null)
            {
                _innerComboBox.PropertyChanged += OnInnerComboBoxPropertyChanged;
                _innerComboBox.SelectionChanged += OnInnerSelectionChanged;
            }
        }

        // ItemsSource and SelectedItem bind independently through ElementName, with no ordering
        // guarantee. If ItemsSource is empty (or momentarily empty - see below) when SelectedItem
        // is set, the inner ComboBox cannot find the item and clears its own selection. The XAML
        // binding on the inner control is one-way (wrapper -> inner only, see OnInnerSelectionChanged
        // below for why), so that spurious clear cannot echo back into the bound view model on its
        // own - but nothing re-tries the selection once the item is findable again either, so that
        // is done explicitly here.
        private INotifyCollectionChanged? _observedItemsSource;

        static ComboBox()
        {
            ItemsSourceProperty.Changed.AddClassHandler<ComboBox>((comboBox, e) => comboBox.OnItemsSourceChanged(e));
        }

        // A bound ObservableCollection whose contents mutate in place (Clear then re-Add, as
        // OutputMappingManager.RebuildEnabledItems does when a slot's IsEnabled toggles) never
        // raises this property's own Changed event - the ItemsSource reference itself never
        // changes, only its contents do, transiently emptying the list along the way. Track the
        // collection's own change notifications too so the selection gets reasserted once it
        // settles, not just when a whole new ItemsSource is assigned.
        private void OnItemsSourceChanged(AvaloniaPropertyChangedEventArgs e) => ResubscribeItemsSource();

        // Detaching and reattaching the control (a side effect of unrelated sibling UI changes)
        // does not raise the ItemsSource property's own Changed event either, so the reattach
        // path below also needs this, reading the current ItemsSource value directly.
        private void ResubscribeItemsSource()
        {
            if (_observedItemsSource != null)
                _observedItemsSource.CollectionChanged -= OnItemsCollectionChanged;

            _observedItemsSource = ItemsSource as INotifyCollectionChanged;
            if (_observedItemsSource != null)
                _observedItemsSource.CollectionChanged += OnItemsCollectionChanged;

            ReassertSelection();
        }

        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => ReassertSelection();

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            ResubscribeItemsSource();
        }

        // The subscription above is otherwise never released: a project-lifetime collection like
        // OutputMappingManager.EnabledItems outlives any one view that binds to it (the panel gets
        // torn down and recreated on every tab switch), so without this the collection keeps this
        // discarded control's handler alive for the rest of the session.
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            if (_observedItemsSource != null)
            {
                _observedItemsSource.CollectionChanged -= OnItemsCollectionChanged;
                _observedItemsSource = null;
            }
        }

        private void ReassertSelection()
        {
            var selected = SelectedItem;
            if (selected == null || _innerComboBox == null) return;
            _innerComboBox.SelectedItem = null;
            _innerComboBox.SelectedItem = selected;
        }

        // The inner ComboBox's SelectedItem binding is intentionally one-way. A plain TwoWay
        // binding echoes every internal reset - including the spurious "item not found in an
        // as-yet-empty ItemsSource" clear described above - straight back up into the bound view
        // model, silently wiping out the real selection. Only a genuine pick (AddedItems non-empty)
        // should propagate outward, so that path is handled explicitly here instead.
        private void OnInnerSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SelectedItem = e.AddedItems[0];
        }

        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<ComboBox, IEnumerable>(nameof(ItemsSource));
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // Optional; when unset the inner ComboBox falls back to its default item rendering
        // (ToString()), which is fine for enum items but does not react to a bound property
        // changing after the item is rendered. Set this when items carry an editable display
        // value (e.g. a Name) so the dropdown stays live.
        public static readonly StyledProperty<IDataTemplate> ItemTemplateProperty =
            AvaloniaProperty.Register<ComboBox, IDataTemplate>(nameof(ItemTemplate));
        public IDataTemplate ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<ComboBox, object?>(nameof(SelectedItem), null, defaultBindingMode: BindingMode.TwoWay);
        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private bool IsComboBoxFocused()
        {
            var focused = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as Visual;
            if (focused == null || _innerComboBox == null)
                return false;

            return focused == _innerComboBox || focused.GetVisualAncestors().Contains(_innerComboBox);
        }

        private void OnInnerPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            bool shouldCycleSelection =
                IsComboBoxFocused() &&
                _innerComboBox != null &&
                !_innerComboBox.IsDropDownOpen &&
                ItemsSource != null;

            if (!shouldCycleSelection)
                return; // leave e.Handled false: bubbles up normally (dropdown scroll, or an ancestor ScrollViewer)

            List<object> items = ItemsSource!.Cast<object>().ToList();
            if (items.Count == 0)
                return;

            int currentIndex = SelectedItem != null ? items.IndexOf(SelectedItem) : -1;
            int newIndex = currentIndex - (int)e.Delta.Y;

            if (newIndex < 0)
                newIndex = 0;
            else if (newIndex >= items.Count)
                newIndex = items.Count - 1;

            if (newIndex != currentIndex)
                SelectedItem = items[newIndex];

            e.Handled = true;
        }

        private void OnInnerPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            _innerComboBox?.Focus();        }

        private void OnInnerComboBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == AvaloniaComboBox.IsDropDownOpenProperty && e.NewValue is false)
                _innerComboBox?.Focus();
        }
    }
}
