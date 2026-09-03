// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    // The local state a peer syncs. All three calls run on the dispatcher thread.
    public interface ISyncTarget
    {
        ProjectModel Capture();

        void ApplySnapshot(ProjectModel model);

        void Apply(IMessage message);
    }
}
