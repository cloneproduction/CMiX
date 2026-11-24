// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Networking
{
    public sealed class ValueSync : IValueSync
    {
        private readonly UndoManager _undoManager;
        private readonly MessageFactory _messageFactory;
        private readonly ControlMessenger _messenger;

        public ValueSync(UndoManager undoManager, MessageFactory messageFactory, ControlMessenger messenger)
        {
            _undoManager = undoManager;
            _messageFactory = messageFactory;
            _messenger = messenger;
        }

        public void Commit(IControl value, IControlModel before, IControlModel after)
        {
            if (!_undoManager.IsApplying)
                _undoManager.Push(new ValueChangedCommand(value, before, after));
            Send(value);
        }

        public void Send(IControl value) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageValueChanged>(value.ID, value));
    }
}
