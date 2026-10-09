// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
    public record MessageValueChanged(Guid ID, IControlModel Value) : IMessage
    {
        public MessageValueChanged() : this(Guid.NewGuid(), default!) { }
    }

    // Push sends this with the full project, so running peers adopt it.
    public record MessageProjectSnapshot(Guid ID, ProjectModel Model) : IMessage;
}
