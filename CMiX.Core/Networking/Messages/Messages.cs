// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Networking.Messages
{
    public record MessageSelectedItemChanged(Guid ID, Guid ControlID, int Index) : IMessageManager;
    public record MessageRemoveSelectedItem(Guid ID) : IMessageManager;
    public record MessageAddItem(Guid ID, IControlModel Model, int SelectedIndex) : IMessageManager;
    public record MessageRemoveItem(Guid ID, Guid ModelID, int SelectedIndex) : IMessageManager;
    public record MessageMoveItem(Guid ID, int OldIndex, int NewIndex) : IMessageManager;
    public record MessageOnClick(Guid ID) : IMessage;
    public record MessageOpenProject(Guid ID, string FilePath) : IMessage;
    public record MessageValueChanged(Guid ID, IControlModel Value) : IMessage
    {
        public MessageValueChanged() : this(Guid.NewGuid(), default!) { }
    }

    // Sent by each side right after a connection is established, carrying a hash of its own
    // current Project.ToModel() output (see ProjectStateHash). Comparing the two tells each side
    // whether it currently holds the same project as the other, without transmitting the whole
    // project just to check.
    public record MessageStateHash(Guid ID, string Hash) : IMessage;

    // The full push/pull payload: one side's complete current project, sent so the other side can
    // discard its own state and rebuild from this instead - real IDs included, so nothing needs to
    // be pre-agreed the way ManagerIDs currently is.
    public record MessageProjectSnapshot(Guid ID, ProjectModel Model) : IMessage;

    // "Pull" is a request-then-reply: this asks the other side to send its current state as a
    // MessageProjectSnapshot. "Push" needs no equivalent request - the sender already has what it
    // wants to send.
    public record MessageRequestSnapshot(Guid ID) : IMessage;
}
