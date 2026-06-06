// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Undo
{
    public class CompositeCommand : IUndoCommand
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
    }
}
