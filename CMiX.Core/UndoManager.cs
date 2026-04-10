// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mapping;
using CMiX.Core.Prefabs;

namespace CMiX.Core
{
    public class UndoManager
    {
        private const int MaxSteps = 32;
        private const int MergeWindowMs = 500;

        private readonly Mapper _mapper;
        private readonly ControlRepository _controlRepository;
        private readonly Dictionary<Guid, WeakReference<IControl>> _controlRegistry = new();
        private readonly Stack<UndoStep> _undoStack = new();
        private readonly Stack<UndoStep> _redoStack = new();

        private bool _isApplying;
        private IControl _pendingControl;
        private IControlModel _pendingBefore;

        public bool IsApplying => _isApplying;
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public UndoManager(Mapper mapper, ControlRepository controlRepository)
        {
            _mapper = mapper;
            _controlRepository = controlRepository;
        }

        public void Register(IControl control)
        {
            _controlRegistry[control.ID] = new WeakReference<IControl>(control);
        }

        public void Record(IControl control)
        {
            _pendingControl = control;
            _pendingBefore = _mapper.MapToModel(control);
        }

        public void Commit(IControl control)
        {
            if (_isApplying) return;
            if (_pendingControl?.ID != control.ID) return;
            Push(new UndoStep(control.ID, _pendingBefore, _mapper.MapToModel(control)));
            _pendingControl = null;
            _pendingBefore = null;
        }

        public void Push(UndoStep step)
        {
            if (_isApplying) return;

            if (CanMerge(step, out var last))
            {
                _undoStack.Pop();
                step = new UndoStep(step.ControlID, last.Before, step.After);
            }

            _undoStack.Push(step);
            _redoStack.Clear();
            TrimStack();
        }

        public void ApplyUndo()
        {
            if (!CanUndo) return;
            var step = _undoStack.Pop();
            _redoStack.Push(step);
            Apply(step, isUndo: true);
        }

        public void ApplyRedo()
        {
            if (!CanRedo) return;
            var step = _redoStack.Pop();
            _undoStack.Push(step);
            Apply(step, isUndo: false);
        }

        private void Apply(UndoStep step, bool isUndo)
        {
            _isApplying = true;
            if (step.UndoAction != null)
            {
                if (isUndo) step.UndoAction();
                else step.RedoAction();
            }
            else
            {
                var model = isUndo ? step.Before : step.After;
                if (_controlRegistry.TryGetValue(step.ControlID, out var weakRef)
                    && weakRef.TryGetTarget(out var control))
                    _mapper.MapToViewModel(control, model);
            }
            _isApplying = false;
        }

        private bool CanMerge(UndoStep step, out UndoStep last)
        {
            last = default;
            return step.UndoAction == null
                && _undoStack.TryPeek(out last)
                && last.UndoAction == null
                && last.ControlID == step.ControlID
                && (step.Timestamp - last.Timestamp).TotalMilliseconds < MergeWindowMs;
        }

        private void TrimStack()
        {
            if (_undoStack.Count <= MaxSteps) return;
            var trimmed = new Stack<UndoStep>(_undoStack.Reverse().Skip(1));
            _undoStack.Clear();
            foreach (var s in trimmed.Reverse())
                _undoStack.Push(s);
        }
    }
}
