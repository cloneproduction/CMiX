// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Networking
{
    // Syncs the running Project. A message goes to the control that answers to its id.
    public sealed class ProjectSyncTarget : ISyncTarget
    {
        private readonly Project _project;
        private readonly ControlMessenger _messenger;
        private readonly UndoManager _undoManager;
        private readonly ControlRegistry _registry;

        public ProjectSyncTarget(Project project, ControlMessenger messenger, UndoManager undoManager, ControlRegistry registry)
        {
            _project = project;
            _messenger = messenger;
            _undoManager = undoManager;
            _registry = registry;
        }

        public ProjectModel Capture() => (ProjectModel)_project.ToModel();

        // A snapshot apply sends no message and records no step. The clear of the stack comes last.
        public void ApplySnapshot(ProjectModel model)
        {
            var wasBlocked = _messenger.IsSendingBlocked;
            _messenger.IsSendingBlocked = true;
            try
            {
                using (_undoManager.Suspend())
                {
                    _project.ApplySnapshot(model);
                    _undoManager.Clear();
                }
            }
            finally
            {
                _messenger.IsSendingBlocked = wasBlocked;
            }
        }

        public void Apply(IMessage message)
        {
            if (message is MessageProjectSnapshot snapshot)
            {
                ApplySnapshot(snapshot.Model);
                return;
            }

            _registry.Find(message.ID)?.Receive(message);
        }
    }
}
