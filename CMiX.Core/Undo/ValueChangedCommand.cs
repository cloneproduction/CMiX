// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class ValueChangedCommand : IUndoCommand
    {
        private readonly IControl _control;
        private IControlModel _before;
        private readonly IControlModel _after;

        public Guid ControlID { get; }
        public DateTime Timestamp { get; }
        public IControlModel Before => _before;

        public ValueChangedCommand(IControl control, IControlModel before, IControlModel after)
        {
            _control = control;
            _before = before;
            _after = after;
            ControlID = control.ID;
            Timestamp = DateTime.Now;
        }

        public ValueChangedCommand WithBefore(IControlModel before)
        {
            _before = before;
            return this;
        }

        public void Execute() => _control.FromModel(_after);
        public void Undo() => _control.FromModel(_before);
    }
}
