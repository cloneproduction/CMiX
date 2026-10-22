// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    // A fixed set of exactly 9 output mappings, one per TexcoordSemantic channel (Texcoord0-8).
    // In theory Texcoord9 should also work but does not for some reason, selecting it resulted in a SharpDX exception.
    // Unlike PrefabManager/CollectionManager the slot count never changes: nothing adds or removes
    // an item, so this class carries none of their add/remove messaging or undo plumbing. Each
    // slot's own fields (Name, Resolution, TexcoordSemantic) are GenericValue<T> and already
    // sync themselves over the network and through undo individually.
    public partial class OutputMappingManager : ObservableObject, IControl
    {
        public const int SlotCount = 9; 

        public OutputMappingManager(ControlFactory controlFactory)
        {
            ID = ManagerIDs.OutputMappingManager;

            var items = new List<OutputMapping>(SlotCount);
            for (int i = 0; i < SlotCount; i++)
                items.Add((OutputMapping)controlFactory.Create(typeof(OutputMapping)));
            Items = items;

            for (int i = 0; i < SlotCount; i++)
                ResetSlot(i);

            SelectedItem = Items[0];
        }

        // Gives every slot the values of a new project. The slot instances stay the same.
        public void Reset()
        {
            for (int i = 0; i < SlotCount; i++)
                ResetSlot(i);
            SelectedItem = Items[0];
        }

        // The ids are assigned before the values change, so each value message carries the fixed id
        // of the slot. OutputMapping.FromModel would send the values with the random ids of the model.
        private void ResetSlot(int i)
        {
            var mapping = Items[i];
            var defaults = new OutputMappingModel();
            AssignFixedIds(mapping, i);
            mapping.Name.Value = $"Output {i + 1}";
            mapping.Resolution.X.Value = defaults.Resolution.X.Value;
            mapping.Resolution.Y.Value = defaults.Resolution.Y.Value;
            mapping.TexcoordSemantic.Value = (TexcoordSemantic)i;
        }

        // Fixed, like ManagerIDs.Project/CompositionManager/etc.: this manager and its 10 slots
        // are created directly at startup on both the .NET and vvvv sides rather than announced
        // through MessageAddItem, so the ID must stay the well-known constant regardless of what a
        // loaded (possibly older) project file says.
        public Guid ID { get; set; } = ManagerIDs.OutputMappingManager;
        public IReadOnlyList<OutputMapping> Items { get; }

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
        }
    }
}
