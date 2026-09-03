// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking
{
    // Syncs the running Project. Messages reach the controls through the messenger they listen to.
    public sealed class ProjectSyncTarget : ISyncTarget
    {
        private readonly Project _project;

        public ProjectSyncTarget(Project project)
        {
            _project = project;
        }

        public ProjectModel Capture() => (ProjectModel)_project.ToModel();

        public void ApplySnapshot(ProjectModel model) => _project.ApplySnapshot(model);

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
