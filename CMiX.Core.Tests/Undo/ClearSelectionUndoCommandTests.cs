// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class ClearSelectionUndoCommandTests
    {
        [Fact]
        public void Execute_WithASelection_SelectsNothing()
        {
            var a = new FakeControl();
            var list = new FakeItemList { Items = { a }, SelectedItem = a };

            new ClearSelectionUndoCommand(list, a, 0).Execute();

            Assert.Null(list.SelectedItem);
            Assert.Equal(new[] { "ClearSelection" }, list.Calls);
        }

        [Fact]
        public void Undo_AfterExecute_SelectsThePreviousItem()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b }, SelectedItem = b };
            var command = new ClearSelectionUndoCommand(list, b, 1);
            command.Execute();

            command.Undo();

            Assert.Same(b, list.SelectedItem);
            Assert.Equal(new[] { "ClearSelection", "SelectAt 1" }, list.Calls);
        }
    }
}
