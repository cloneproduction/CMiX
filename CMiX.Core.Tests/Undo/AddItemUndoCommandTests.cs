// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class AddItemUndoCommandTests
    {
        [Fact]
        public void Execute_AnIndexPastTheEnd_InsertsTheItemAtTheEndAndSelectsIt()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var added = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = b };

            new AddItemUndoCommand(list, added, 5).Execute();

            Assert.Equal(new IControl[] { a, b, added }, list.Items);
            Assert.Same(added, list.SelectedItem);
            Assert.Equal(new[] { "Insert 2" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterExecute_RemovesTheItemAndSelectsThePreviousItem()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var added = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = b };
            var command = new AddItemUndoCommand(list, added, 0);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b }, list.Items);
            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "Insert 0", "Remove", "SelectAt 1" }, list.Calls);
        }

        [Fact]
        public void Undo_WithNothingSelectedBefore_SelectsNothingNew()
        {
            var added = new FakeControl();
            var list = new FakeItemList();
            var command = new AddItemUndoCommand(list, added, 0);
            command.Execute();

            command.Undo();

            Assert.Empty(list.Items);
            Assert.Null(list.SelectedItem);
            Assert.Equal(new[] { "Insert 0", "Remove" }, list.Calls);
        }

        [Fact]
        public void Dispose_AfterUndo_ReleasesTheItem()
        {
            var added = new FakeControl();
            var list = new FakeItemList();
            var command = new AddItemUndoCommand(list, added, 0);
            command.Execute();
            command.Undo();

            command.Dispose();

            Assert.Equal(new IControl[] { added }, list.Released);
        }
    }
}
