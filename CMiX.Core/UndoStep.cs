// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core
{
    public class UndoStep
    {
        public Guid ControlID { get; init; }
        public IControlModel Before { get; init; }
        public IControlModel After { get; init; }
        public DateTime Timestamp { get; init; }
        public Action UndoAction { get; init; }
        public Action RedoAction { get; init; }

        // for value changes
        public UndoStep(Guid controlID, IControlModel before, IControlModel after)
        {
            ControlID = controlID;
            Before = before;
            After = after;
            Timestamp = DateTime.Now;
        }

        // for collection operations
        public UndoStep(Guid controlID, Action undoAction, Action redoAction)
        {
            ControlID = controlID;
            UndoAction = undoAction;
            RedoAction = redoAction;
            Timestamp = DateTime.Now;
        }
    }
}
