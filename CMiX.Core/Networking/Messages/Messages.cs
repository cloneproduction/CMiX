// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Networking.Messages
{
    public record MessageSelectedItemChanged(Guid ID, Guid ControlID, int Index) : IMessageManager;
    public record MessageRemoveSelectedItem(Guid ID) : IMessageManager;
    public record MessageAddItem(Guid ID, IControlModel Model, int SelectedIndex) : IMessageManager;
    public record MessageReplaceItem(Guid ID, IControlModel ControlModel, int Index, int SelectedIndex) : IMessageManager;
    public record MessageRemoveItem(Guid ID, Guid ModelID, int SelectedIndex) : IMessageManager;
    public record MessageMoveItem(Guid ID, int OldIndex, int NewIndex) : IMessageManager;
    public record MessageOnClick(Guid ID) : IMessage;
    public record MessageOpenProject(Guid ID, string FilePath) : IMessage;
    public record MessageValueChanged(Guid ID, IControlModel Value) : IMessage
    {
        public MessageValueChanged() : this(Guid.NewGuid(), default!) { }
    }
}
