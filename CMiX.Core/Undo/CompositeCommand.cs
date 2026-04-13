// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Undo
{
    public class CompositeCommand : IUndoCommand
    {
        private readonly List<IUndoCommand> _commands = new();

        public void Add(IUndoCommand command) => _commands.Add(command);

        public void Execute()
        {
            foreach (var command in _commands)
                command.Execute();
        }

        public void Undo()
        {
            foreach (var command in _commands.AsEnumerable().Reverse())
                command.Undo();
        }
    }
}
