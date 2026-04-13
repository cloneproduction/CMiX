// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Undo
{
    public class UndoManager
    {
        private const int MaxSteps = 32;
        private const int MergeWindowMs = 500;

        private readonly Stack<IUndoCommand> _undoStack = new();
        private readonly Stack<IUndoCommand> _redoStack = new();
        private int _groupDepth = 0;

        public bool IsApplying { get; private set; }
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void BeginGroup() => _groupDepth++;
        public void EndGroup() => _groupDepth = Math.Max(0, _groupDepth - 1);

        public void Push(IUndoCommand command)
        {
            if (_groupDepth > 0) return;

            if (command is ValueChangedCommand vc
                && _undoStack.TryPeek(out var last)
                && last is ValueChangedCommand lastVc
                && lastVc.ControlID == vc.ControlID
                && (vc.Timestamp - lastVc.Timestamp).TotalMilliseconds < MergeWindowMs)
            {
                _undoStack.Pop();
                _undoStack.Push(vc.WithBefore(lastVc.Before));
                return;
            }

            _undoStack.Push(command);
            _redoStack.Clear();
            TrimStack();
        }

        public void Undo()
        {
            if (!CanUndo) return;
            IsApplying = true;
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            IsApplying = false;
        }

        public void Redo()
        {
            if (!CanRedo) return;
            IsApplying = true;
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            IsApplying = false;
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        private void TrimStack()
        {
            while (_undoStack.Count > MaxSteps)
            {
                var trimmed = new Stack<IUndoCommand>(_undoStack.Reverse().Skip(1));
                _undoStack.Clear();
                foreach (var s in trimmed.Reverse())
                    _undoStack.Push(s);
            }
        }
    }
}
