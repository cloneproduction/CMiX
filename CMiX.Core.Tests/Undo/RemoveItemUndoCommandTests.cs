// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class RemoveItemUndoCommandTests
    {
        [Fact]
        public void Execute_TheSelectedItem_RemovesItAndSelectsTheItemBefore()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c }, SelectedItem = c };

            new RemoveItemUndoCommand(list, c).Execute();

            Assert.Equal(new IControl[] { a, b }, list.Items);
            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "Remove" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterRemovingAnotherItem_InsertsTheItemAndSelectsTheFormerSelection()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c }, SelectedItem = a };
            var command = new RemoveItemUndoCommand(list, b);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b, c }, list.Items);
            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 1", "SelectAt 0" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterRemovingTheSelectedItem_SelectsTheItemAgain()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = b };
            var command = new RemoveItemUndoCommand(list, b);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b }, list.Items);
            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 1" }, list.Calls);
        }

        [Fact]
        public void Undo_WithNothingSelectedBefore_KeepsTheRestoredItemSelected()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b } };
            var command = new RemoveItemUndoCommand(list, a);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b }, list.Items);
            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Remove", "Insert 0" }, list.Calls);
        }

        [Fact]
        public void Dispose_AfterExecute_ReleasesTheItem()
        {
            var a = new FakeControl();
            var list = new FakeItemList { Items = { a } };
            var command = new RemoveItemUndoCommand(list, a);
            command.Execute();

            command.Dispose();

            Assert.Equal(new IControl[] { a }, list.Released);
        }
    }
}
