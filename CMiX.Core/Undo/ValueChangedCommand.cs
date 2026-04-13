// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
