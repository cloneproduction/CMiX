// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class CompositeCommand : IUndoCommand, IDisposable
    {
        private readonly List<IUndoCommand> _commands;

        public CompositeCommand(List<IUndoCommand> commands)
        {
            _commands = commands;
        }

        public void Execute()
        {
            foreach (var cmd in _commands)
                cmd.Execute();
        }

        public void Undo()
        {
            foreach (var cmd in _commands.AsEnumerable().Reverse())
                cmd.Undo();
        }

        // Ownership of the removed controls sits on the grouped commands, so dropping the group
        // has to hand the disposal down to them.
        public void Dispose()
        {
            foreach (var cmd in _commands)
                if (cmd is IDisposable disposable) disposable.Dispose();
        }
    }
}
