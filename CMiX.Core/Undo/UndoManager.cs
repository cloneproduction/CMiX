// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Undo
{
    public class UndoManager
    {
        private const int MaxSteps = 128;
        private const int MergeWindowMs = 500;

        // A linked list rather than a stack because the cap is enforced by dropping the oldest
        // entry, which costs one node removal here and a full rebuild on a stack. The newest
        // entry is the last node, so pushing, peeking and popping all stay at the tail.
        private readonly LinkedList<IUndoCommand> _undoStack = new();
        private readonly Stack<IUndoCommand> _redoStack = new();
        private int _groupDepth = 0;

        public bool IsApplying { get; private set; }
        public bool IsSuppressed => _groupDepth > 0;
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void SuppressUndo() => _groupDepth++;
        public void ResumeUndo() => _groupDepth = Math.Max(0, _groupDepth - 1);

        public void Push(IUndoCommand command)
        {
            if (_groupDepth > 0) return;

            if (_captureList != null)
            {
                _captureList.Add(command);
                return;
            }

            if (command is ValueChangedCommand vc
                && _undoStack.Last is { } lastNode
                && lastNode.Value is ValueChangedCommand lastVc
                && lastVc.ControlID == vc.ControlID
                && (vc.Timestamp - lastVc.Timestamp).TotalMilliseconds < MergeWindowMs)
            {
                lastNode.Value = vc.WithBefore(lastVc.Before);
                _redoStack.Clear();
                return;
            }

            _undoStack.AddLast(command);
            _redoStack.Clear();
            TrimStack();
        }

        private List<IUndoCommand> _captureList = null;

        public void BeginCapture()
        {
            _captureList = new List<IUndoCommand>();
        }

        public void EndCapture()
        {
            if (_captureList == null) return;
            var commands = _captureList;
            _captureList = null;
            if (commands.Count == 0) return;
            if (commands.Count == 1) { Push(commands[0]); return; }
            _undoStack.AddLast(new CompositeCommand(commands));
            _redoStack.Clear();
            TrimStack();
        }

        public void Undo()
        {
            if (!CanUndo) return;
            IsApplying = true;
            var command = _undoStack.Last.Value;
            _undoStack.RemoveLast();
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
            _undoStack.AddLast(command);
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
                _undoStack.RemoveFirst();
        }
    }
}
