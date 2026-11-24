// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class MoveItemUndoCommandTests
    {
        [Fact]
        public void Execute_TwoIndexes_MovesTheItemAndKeepsTheSelection()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c }, SelectedItem = a };

            new MoveItemUndoCommand(list, 0, 2).Execute();

            Assert.Equal(new IControl[] { b, c, a }, list.Items);
            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Move 0 2" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterExecute_MovesTheItemBack()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var c = new FakeControl();
            var list = new FakeItemList { Items = { a, b, c }, SelectedItem = a };
            var command = new MoveItemUndoCommand(list, 0, 2);
            command.Execute();

            command.Undo();

            Assert.Equal(new IControl[] { a, b, c }, list.Items);
            Assert.Same(a, list.SelectedItem);
            Assert.Equal(new[] { "Move 0 2", "Move 2 0" }, list.Calls);
        }
    }
}
