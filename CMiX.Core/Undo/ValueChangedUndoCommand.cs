// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class ValueChangedUndoCommand : IUndoCommand
    {
        private readonly IControl _control;
        private IControlModel _before;
        private readonly IControlModel _after;

        public Guid ControlID { get; }
        public IControlModel Before => _before;
        // The timestamp of the time provider when the manager pushed this step.
        public long PushedAt { get; set; }

        public ValueChangedUndoCommand(IControl control, IControlModel before, IControlModel after)
        {
            _control = control;
            _before = before;
            _after = after;
            ControlID = control.ID;
        }

        public void MergeBefore(IControlModel before) => _before = before;

        public void Execute() => _control.FromModel(_after);
        public void Undo() => _control.FromModel(_before);
    }
}
