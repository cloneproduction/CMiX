// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class UndoManager
    {
        public const int MaxSteps = 128;

        private readonly TimeProvider _time;
        // Two value steps of one control inside this window become one step.
        private readonly long _mergeWindowTicks;

        // A linked list rather than a stack because the cap is enforced by dropping the oldest
        // entry, which costs one node removal here and a full rebuild on a stack. The newest
        // entry is the last node, so pushing, peeking and popping all stay at the tail.
        private readonly LinkedList<IUndoCommand> _undoStack = new();
        private readonly Stack<IUndoCommand> _redoStack = new();
        private int _groupDepth = 0;
        private readonly List<IUndoCommand> _captured = new();
        private readonly Stack<int> _captureStarts = new();

        public bool IsApplying { get; private set; }
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
        public int StepCount => _undoStack.Count;
        public int RedoStepCount => _redoStack.Count;

        public UndoManager(TimeProvider time)
        {
            _time = time;
            _mergeWindowTicks = time.TimestampFrequency / 2;
        }

        // Fires once after each public operation that changes the undo or the redo stack.
        public event Action Changed;

        public void SuppressUndo() => _groupDepth++;
        public void ResumeUndo() => _groupDepth = Math.Max(0, _groupDepth - 1);

        public SuspendScope Suspend()
        {
            SuppressUndo();
            return new SuspendScope(this);
        }

        public CaptureScope Capture()
        {
            BeginCapture();
            return new CaptureScope(this, _captureStarts.Count);
        }

        // Runs the action as one step of the control. The value steps inside the action are not recorded.
        public void RecordAsOneStep(IControl control, Action action)
        {
            var before = control.ToModel();
            using (Suspend())
                action();
            Push(new ValueChangedUndoCommand(control, before, control.ToModel()));
        }

        public void Push(IUndoCommand command)
        {
            if (IsApplying || _groupDepth > 0) return;

            if (_captureStarts.Count > 0)
            {
                _captured.Add(command);
                return;
            }

            var now = _time.GetTimestamp();
            if (command is ValueChangedUndoCommand vc
                && _undoStack.Last is { } lastNode
                && lastNode.Value is ValueChangedUndoCommand lastVc
                && lastVc.ControlID == vc.ControlID
                && now - lastVc.PushedAt < _mergeWindowTicks)
            {
                vc.MergeBefore(lastVc.Before);
                vc.PushedAt = now;
                lastNode.Value = vc;
                DropRedoStack();
                RaiseChanged();
                return;
            }

            if (command is ValueChangedUndoCommand pushed)
                pushed.PushedAt = now;
            _undoStack.AddLast(command);
            DropRedoStack();
            TrimStack();
            RaiseChanged();
        }

        // Nestable: only the outermost EndCapture pushes the captured commands as one step.
        public void BeginCapture() => _captureStarts.Push(_captured.Count);

        public void EndCapture() => EndCaptureAt(_captureStarts.Count);

        // Ends the capture and undoes, newest first, what it captured since its BeginCapture. Nothing goes on the undo stack.
        public void RevertCapture() => RevertCaptureAt(_captureStarts.Count);

        // A depth above the open captures means that the capture of this depth is already reverted.
        private void EndCaptureAt(int depth)
        {
            if (_captureStarts.Count == 0 || _captureStarts.Count < depth) return;
            _captureStarts.Pop();
            if (_captureStarts.Count > 0) return;

            var commands = new List<IUndoCommand>(_captured);
            _captured.Clear();
            if (commands.Count == 0) return;
            if (commands.Count == 1) { Push(commands[0]); return; }
            _undoStack.AddLast(new CompositeUndoCommand(commands));
            DropRedoStack();
            TrimStack();
            RaiseChanged();
        }

        private void RevertCaptureAt(int depth)
        {
            if (_captureStarts.Count == 0 || _captureStarts.Count < depth) return;
            var start = _captureStarts.Pop();
            var reverted = _captured.GetRange(start, _captured.Count - start);
            _captured.RemoveRange(start, reverted.Count);

            IsApplying = true;
            try
            {
                for (var i = reverted.Count - 1; i >= 0; i--)
                    reverted[i].Undo();
            }
            finally
            {
                IsApplying = false;
            }

            foreach (var command in reverted)
                DropCommand(command);
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
            RaiseChanged();
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
            RaiseChanged();
        }

        public void Clear()
        {
            foreach (var command in _undoStack)
                DropCommand(command);
            _undoStack.Clear();
            DropRedoStack();
            RaiseChanged();
        }

        private void RaiseChanged() => Changed?.Invoke();

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

        public readonly struct CaptureScope : IDisposable
        {
            private readonly UndoManager _owner;
            private readonly int _depth;

            internal CaptureScope(UndoManager owner, int depth)
            {
                _owner = owner;
                _depth = depth;
            }

            public void Revert() => _owner.RevertCaptureAt(_depth);

            public void Dispose() => _owner.EndCaptureAt(_depth);
        }

        public readonly struct SuspendScope : IDisposable
        {
            private readonly UndoManager _owner;

            internal SuspendScope(UndoManager owner) => _owner = owner;

            public void Dispose() => _owner.ResumeUndo();
        }
    }
}
