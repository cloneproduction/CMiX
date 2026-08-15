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
                DropRedoStack();
                return;
            }

            _undoStack.AddLast(command);
            DropRedoStack();
            TrimStack();
        }

        private List<IUndoCommand> _captureList = null;
        private int _captureDepth = 0;

        // Nestable so a capture opened by one caller (for example DeleteEverywhere) survives a
        // second capture opened by code it calls into (for example a per file loop). Only the
        // outermost BeginCapture starts a new list, and only the outermost EndCapture flushes it.
        public void BeginCapture()
        {
            if (_captureDepth == 0)
                _captureList = new List<IUndoCommand>();
            _captureDepth++;
        }

        public void EndCapture()
        {
            if (_captureDepth == 0) return;
            _captureDepth--;
            if (_captureDepth > 0) return;

            if (_captureList == null) return;
            var commands = _captureList;
            _captureList = null;
            if (commands.Count == 0) return;
            if (commands.Count == 1) { Push(commands[0]); return; }
            _undoStack.AddLast(new CompositeCommand(commands));
            DropRedoStack();
            TrimStack();
        }

        public void Undo()
        {
            if (!CanUndo) return;
            var command = _undoStack.Last.Value;
            _undoStack.RemoveLast();

            // IsApplying gates undo recording across the whole app, so it has to be cleared even
            // when the command throws, otherwise nothing is ever recorded again.
            IsApplying = true;
            try
            {
                command.Undo();
            }
            finally
            {
                IsApplying = false;
            }

            _redoStack.Push(command);
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var command = _redoStack.Pop();

            IsApplying = true;
            try
            {
                command.Execute();
            }
            finally
            {
                IsApplying = false;
            }

            _undoStack.AddLast(command);
        }

        public void Clear()
        {
            foreach (var command in _undoStack)
                DropCommand(command);
            _undoStack.Clear();
            DropRedoStack();
        }

        private void TrimStack()
        {
            while (_undoStack.Count > MaxSteps)
            {
                DropCommand(_undoStack.First.Value);
                _undoStack.RemoveFirst();
            }
        }

        private void DropRedoStack()
        {
            foreach (var command in _redoStack)
                DropCommand(command);
            _redoStack.Clear();
        }

        // Dropping a command is the point where a delete becomes permanent, so a command that
        // owns a removed control disposes it here. Recording is suppressed for the duration
        // because a control teardown can touch selections that would otherwise push new commands
        // while this one is being dropped.
        private void DropCommand(IUndoCommand command)
        {
            if (command is not IDisposable disposable) return;

            SuppressUndo();
            try
            {
                disposable.Dispose();
            }
            finally
            {
                ResumeUndo();
            }
        }
    }
}
