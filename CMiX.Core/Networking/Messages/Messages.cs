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

    // Sent on connect so each side can compare its own project hash against the other's.
    public record MessageStateHash(Guid ID, string Hash) : IMessage;

    // The push/pull payload: one side's full project, real IDs included, for the other side to
    // adopt wholesale.
    public record MessageProjectSnapshot(Guid ID, ProjectModel Model) : IMessage;

    // Asks the other side to reply with a MessageProjectSnapshot ("pull").
    public record MessageRequestSnapshot(Guid ID) : IMessage;
}
