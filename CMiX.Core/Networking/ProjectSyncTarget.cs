// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking
{
    // Syncs the running Project. Messages reach the controls through the messenger they listen to.
    public sealed class ProjectSyncTarget : ISyncTarget
    {
        private readonly Project _project;
        private readonly ControlMessenger _messenger;
        private readonly UndoManager _undoManager;

        public ProjectSyncTarget(Project project, ControlMessenger messenger, UndoManager undoManager)
        {
            _project = project;
            _messenger = messenger;
            _undoManager = undoManager;
        }

        public ProjectModel Capture() => (ProjectModel)_project.ToModel();

        // The clear and the load send messages and record undo steps. A snapshot apply must do
        // neither: the messages would go back into the stream, and the steps would undo into the
        // state the snapshot replaced. The clear of the stack comes last, as in MainMenu.
        public void ApplySnapshot(ProjectModel model)
        {
            var wasBlocked = _messenger.IsSendingBlocked;
            _messenger.IsSendingBlocked = true;
            _undoManager.SuppressUndo();
            try
            {
                _project.ApplySnapshot(model);
                _undoManager.Clear();
            }
            finally
            {
                _undoManager.ResumeUndo();
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

            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
