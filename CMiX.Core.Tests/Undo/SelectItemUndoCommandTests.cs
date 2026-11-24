// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class SelectItemUndoCommandTests
    {
        [Fact]
        public void Execute_AnIndexInTheList_SelectsTheItem()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = a };

            new SelectItemUndoCommand(list, a, 0, 1).Execute();

            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "SelectAt 1" }, list.Calls);
        }

        [Fact]
        public void Execute_AnIndexOutOfRange_ChangesNothing()
        {
            var a = new FakeControl();
            var list = new FakeItemList { Items = { a }, SelectedItem = a };

            new SelectItemUndoCommand(list, a, 0, 3).Execute();

            Assert.Same(a, list.SelectedItem);
            Assert.Empty(list.Calls);
        }

        [Fact]
        public void Undo_AfterExecute_SelectsThePreviousItem()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = a };
            var command = new SelectItemUndoCommand(list, a, 0, 1);
            command.Execute();

            command.Undo();

            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "SelectAt 1", "SelectAt 0" }, list.Calls);
        }

        [Fact]
        public void Undo_APreviousIndexOutOfRange_ChangesNothing()
        {
            var a = new FakeControl();
            var list = new FakeItemList { Items = { a }, SelectedItem = a };

            new SelectItemUndoCommand(list, null, -1, 0).Undo();

            Assert.Same(a, list.SelectedItem);
            Assert.Empty(list.Calls);
        }
    }
}
