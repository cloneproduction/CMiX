// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;
using Xunit;
using FakeControl = CMiX.Core.Tests.FakeItemList.FakeControl;

namespace CMiX.Core.Tests.Undo
{
    public class CompositeUndoCommandTests
    {
        [Fact]
        public void Dispose_TwoOwningSteps_ReleasesBothItems()
        {
            var a = new FakeControl();
            var b = new FakeControl();
            var list = new FakeItemList { Items = { a, b } };
            var removeA = new RemoveItemUndoCommand(list, a);
            var removeB = new RemoveItemUndoCommand(list, b);
            removeA.Execute();
            removeB.Execute();
            var composite = new CompositeUndoCommand(new List<IUndoCommand> { removeA, removeB });

            composite.Dispose();

            Assert.Equal(new IControl[] { a, b }, list.Released);
        }

        [Fact]
        public void Undo_TwoSteps_UndoesThemNewestFirst()
        {
            var log = new List<string>();
            var composite = new CompositeUndoCommand(new List<IUndoCommand> { new RecordingUndoCommand(1, log), new RecordingUndoCommand(2, log) });
            composite.Execute();

            composite.Undo();

            Assert.Equal(new[] { "Execute 1", "Execute 2", "Undo 2", "Undo 1" }, log);
        }

        private sealed class RecordingUndoCommand : IUndoCommand
        {
            private readonly int _id;
            private readonly List<string> _log;

            public RecordingUndoCommand(int id, List<string> log)
            {
                _id = id;
                _log = log;
            }

            public void Execute() => _log.Add($"Execute {_id}");
            public void Undo() => _log.Add($"Undo {_id}");
        }
    }
}
