// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

// ManagerIDs lives in the CMiX.Core root namespace.
using CMiX.Core;

namespace CMiX.Core.Rendering
{
    // A fixed set of exactly 10 output mappings, one per TexcoordSemantic channel (Texcoord0-9).
    // Unlike PrefabManager/CollectionManager the slot count never changes: nothing adds or removes
    // an item, so this class carries none of their add/remove messaging or undo plumbing. Each
    // slot's own fields (Resolution, TexcoordSemantic, Visibility) are GenericValue<T> and already
    // sync themselves over the network and through undo individually.
    public partial class OutputMappingManager : ObservableObject, IControl
    {
        public const int SlotCount = 10;

        public OutputMappingManager(ControlFactory controlFactory)
        {
            ID = ManagerIDs.OutputMappingManager;

            var items = new List<OutputMapping>(SlotCount);
            for (int i = 0; i < SlotCount; i++)
            {
                var mapping = (OutputMapping)controlFactory.Create(typeof(OutputMapping));
                AssignFixedIds(mapping, i);
                mapping.Name.Value = $"Output {i + 1}";
                mapping.TexcoordSemantic.Value = (TexcoordSemantic)i;
                items.Add(mapping);
            }
            Items = items;

            foreach (var mapping in Items)
                mapping.Visibility.PropertyChanged += OnMappingVisibilityChanged;

            RebuildVisibleItems();
            SelectedItem = Items[0];
        }

        // Fixed, like ManagerIDs.Project/CompositionManager/etc.: this manager and its 10 slots
        // are created directly at startup on both the .NET and vvvv sides rather than announced
        // through MessageAddItem, so the ID must stay the well-known constant regardless of what a
        // loaded (possibly older) project file says.
        public Guid ID { get; set; } = ManagerIDs.OutputMappingManager;
        public IReadOnlyList<OutputMapping> Items { get; }

        // Live view of the slots currently visible, kept in sync as Visibility toggles. This is what
        // a Composition's output mapping ComboBox should bind its ItemsSource to.
        public ObservableCollection<OutputMapping> VisibleItems { get; } = new();

        public OutputMapping GetByID(Guid id) => Items.FirstOrDefault(m => m.ID == id);

        // Which slot the project-level editor panel currently shows. Local-only navigation state,
        // not networked or undoable, matching ManagerData.SelectedIndex.
        [ObservableProperty]
        private OutputMapping selectedItem;

        // Re-stamps a slot and every one of its nested GenericValue<T> fields with the fixed
        // ManagerIDs for that slot index. Needed both right after ControlFactory.Create (which runs
        // FromModel against a fresh default model, handing out random Guids) and after loading a
        // saved project (whose OutputMapping.FromModel/GenericValue.FromModel calls do the same).
        private static void AssignFixedIds(OutputMapping mapping, int slotIndex)
        {
            mapping.ID = ManagerIDs.OutputMappingSlots[slotIndex];
            var fieldIds = ManagerIDs.OutputMappingSlotFields[slotIndex];
            mapping.Name.ID = fieldIds.Name;
            mapping.Resolution.ID = fieldIds.Resolution;
            mapping.Resolution.X.ID = fieldIds.ResolutionX;
            mapping.Resolution.Y.ID = fieldIds.ResolutionY;
            mapping.TexcoordSemantic.ID = fieldIds.TexcoordSemantic;
            mapping.Visibility.ID = fieldIds.Visibility;
        }

        private void OnMappingVisibilityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(GenericValue<bool>.Value)) return;
            var mapping = Items.First(m => ReferenceEquals(m.Visibility, sender));
            UpdateVisibleItem(mapping);
        }

        // Toggling a single slot must never touch VisibleItems wholesale: ObservableCollection.Clear
        // raises a Reset notification, and every standard ItemsControl/ComboBox treats Reset as "I
        // don't know what changed" and defensively drops its current selection - even when the
        // selected item was never one of the ones removed. Add/Remove on the one slot that actually
        // changed lets the bound ComboBox tell the difference and leaves an unrelated selection alone.
        private void UpdateVisibleItem(OutputMapping mapping)
        {
            if (mapping.Visibility.Value)
            {
                if (VisibleItems.Contains(mapping)) return;
                int insertIndex = Items.TakeWhile(m => m != mapping).Count(VisibleItems.Contains);
                VisibleItems.Insert(insertIndex, mapping);
            }
            else
            {
                VisibleItems.Remove(mapping);
            }
        }

        // Full rebuild, only used where there is no meaningful existing selection to protect:
        // initial construction and loading a project file.
        private void RebuildVisibleItems()
        {
            VisibleItems.Clear();
            foreach (var mapping in Items)
                if (mapping.Visibility.Value)
                    VisibleItems.Add(mapping);
        }

        public IControlModel ToModel() => new OutputMappingManagerModel
        {
            ID = ID,
            Items = Items.Select(m => (OutputMappingModel)m.ToModel()).ToList()
        };

        public void FromModel(IControlModel model)
        {
            // ID intentionally left untouched: fixed to ManagerIDs.OutputMappingManager regardless
            // of what an (older) saved file contains.
            var m = (OutputMappingManagerModel)model;
            for (int i = 0; i < Items.Count && i < m.Items.Count; i++)
            {
                Items[i].FromModel(m.Items[i]);
                AssignFixedIds(Items[i], i);
            }
            RebuildVisibleItems();
        }
    }
}
