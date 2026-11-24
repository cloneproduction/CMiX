// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;

namespace CMiX.Core.Tests
{
    // An item list for tests. It follows the selection rules of ManagerData and records each write verb in Calls.
    internal sealed class FakeItemList : IItemList
    {
        public List<IControl> Items { get; } = new();

        public IControl SelectedItem { get; set; }

        public List<string> Calls { get; } = new();

        public List<IControl> Released { get; } = new();

        public int Count => Items.Count;

        public int SelectedIndex => SelectedItem == null ? -1 : Items.IndexOf(SelectedItem);

        public int IndexOf(IControl item) => Items.IndexOf(item);

        public void Insert(IControl item, int index)
        {
            Calls.Add($"Insert {index}");
            Items.Insert(Math.Min(index, Items.Count), item);
            SelectedItem = item;
        }

        public int Remove(IControl item)
        {
            Calls.Add("Remove");
            var index = Items.IndexOf(item);
            if (index < 0) return -1;

            if (ReferenceEquals(item, SelectedItem))
                SelectedItem = Items.Count == 1 ? null : Items[index == 0 ? 1 : index - 1];
            Items.RemoveAt(index);
            return index;
        }

        public void Move(int oldIndex, int newIndex)
        {
            Calls.Add($"Move {oldIndex} {newIndex}");
            if (oldIndex < 0 || oldIndex >= Items.Count) return;
            if (newIndex < 0 || newIndex >= Items.Count) return;

            var item = Items[oldIndex];
            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, item);
        }

        public void SelectAt(int index)
        {
            Calls.Add($"SelectAt {index}");
            SelectedItem = index >= 0 && index < Items.Count ? Items[index] : null;
        }

        public void ClearSelection()
        {
            Calls.Add("ClearSelection");
            SelectedItem = null;
        }

        public void Release(IControl item)
        {
            Calls.Add("Release");
            Released.Add(item);
        }

        public sealed class FakeControl : IControl
        {
            public Guid ID { get; set; } = Guid.NewGuid();

            public IControlModel ToModel() => throw new NotSupportedException();

            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }
    }
}
