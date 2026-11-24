// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class ReplaceItemUndoCommandTests
    {
        [Fact]
        public void Execute_AnItemInTheMiddle_PutsTheNewItemAtItsIndex()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var fresh = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c } };

            new ReplaceItemUndoCommand(list, b, fresh, 1).Execute();

            Assert.Equal(new IControl[] { a, fresh, c }, list.Items);
            Assert.Same(fresh, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 1" }, list.Calls);
        }

        [Fact]
        public void Execute_AnItemNotInTheList_InsertsTheNewItemAtTheEnd()
        {
            var a = new FakeControl();
            var absent = new FakeControl();
            var fresh = new FakeControl();
            var list = new FakeItemList { Items = { a } };

            new ReplaceItemUndoCommand(list, absent, fresh, 5).Execute();

            Assert.Equal(new IControl[] { a, fresh }, list.Items);
            Assert.Equal(new[] { "Remove", "Insert 1" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterExecute_PutsThePreviousItemBack()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var fresh = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c } };
            var command = new ReplaceItemUndoCommand(list, b, fresh, 1);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b, c }, list.Items);
            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 1", "Remove", "Insert 1" }, list.Calls);
        }

        [Fact]
        public void Dispose_AfterExecute_ReleasesBothItems()
        {
            var a = new FakeControl();
            var fresh = new FakeControl();
            var list = new FakeItemList { Items = { a } };
            var command = new ReplaceItemUndoCommand(list, a, fresh, 0);
            command.Execute();

            command.Dispose();

            Assert.Equal(new IControl[] { a, fresh }, list.Released);
        }
    }
}
