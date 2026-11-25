// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class ItemListUndoCommandTests
    {
        [Fact]
        public void Undo_WithNothingSelectedBefore_SelectsNothing()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b } };
            var step = new InsertStep(list, c, 1);
            step.Execute();

            step.Undo();

            Assert.Equal(new IControl[] { a, b }, list.Items);
            Assert.Null(list.SelectedItem);
            Assert.Equal(new[] { "Insert 1", "Remove", "ClearSelection" }, list.Calls);
        }

        [Fact]
        public void Undo_WithAnotherItemSelectedBefore_SelectsItAgain()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = b };
            var step = new InsertStep(list, c, 1);
            step.Execute();

            step.Undo();

            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "Insert 1", "Remove", "SelectAt 1" }, list.Calls);
        }

        [Fact]
        public void Execute_ASecondTime_RestoresTheSelectionAfterTheFirstExecute()
        {
            // The second Remove keeps c selected, because b is not selected. Only the restore selects a.
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c }, SelectedItem = a };
            var step = new RemoveStep(list, b);
            step.Execute();
            step.Undo();
            list.SelectAt(2);

            step.Execute();

            Assert.Equal(new IControl[] { a, c }, list.Items);
            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 1", "SelectAt 0", "SelectAt 2", "Remove", "SelectAt 0" }, list.Calls);
        }

        [Fact]
        public void Undo_WhenTheListAlreadyShowsTheSelection_CallsNoSelect()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = a };
            var step = new InsertStep(list, c, 1);
            step.Execute();

            step.Undo();

            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Insert 1", "Remove" }, list.Calls);
        }

        private sealed class InsertStep : ItemListUndoCommand
        {
            private readonly IControl _item;
            private readonly int _index;

            public InsertStep(IItemList list, IControl item, int index) : base(list)
            {
                _item = item;
                _index = index;
            }

            protected override void Change() => List.Insert(_item, _index);

            protected override void ChangeBack() => List.Remove(_item);
        }

        private sealed class RemoveStep : ItemListUndoCommand
        {
            private readonly IControl _item;
            private int _index;

            public RemoveStep(IItemList list, IControl item) : base(list) => _item = item;

            protected override void Change() => _index = List.Remove(_item);

            protected override void ChangeBack() => List.Insert(_item, _index);
        }
    }
}
