// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Networking
{
    public sealed class ValueSync : IValueSync, IInteractiveValue
    {
        private readonly UndoManager _undoManager;
        private readonly MessageFactory _messageFactory;
        private readonly ControlMessenger _messenger;
        private readonly ValueInteraction _interaction;
        private readonly TimeProvider _time;
        private readonly long _sendIntervalTicks;

        private IControl _value;
        private IControlModel _before;
        private bool _inInteraction;
        private long _lastSendTicks;
        private bool _sendPending;

        public ValueSync(UndoManager undoManager, MessageFactory messageFactory, ControlMessenger messenger, ValueInteraction interaction, TimeProvider time)
        {
            _undoManager = undoManager;
            _messageFactory = messageFactory;
            _messenger = messenger;
            _interaction = interaction;
            _time = time;
            _sendIntervalTicks = time.TimestampFrequency / 60;
        }

        public void Changing(IControl value)
        {
            _value = value;
            // In a drag the model before the first write is the before of the undo step.
            if (_interaction.IsActive && _inInteraction) return;
            _before = _value.ToModel();
        }

        public void Changed(IControl value)
        {
            _value = value;
            if (_interaction.IsActive)
            {
                WriteInInteraction();
                return;
            }

            Record(_before, _value.ToModel());
            _before = null;
            SendNow();
        }

        public void Send(IControl value)
        {
            _value = value;
            if (_interaction.IsActive)
            {
                _before = null;
                WriteInInteraction();
                return;
            }

            SendNow();
        }

        // A write in a ValueInteraction scope rate limits the messages. EndInteraction records the undo step.
        private void WriteInInteraction()
        {
            var now = _time.GetTimestamp();
            if (!_inInteraction)
            {
                _inInteraction = true;
                _lastSendTicks = now - _sendIntervalTicks;
                _sendPending = false;
                _interaction.Enlist(this);
            }

            if (now - _lastSendTicks >= _sendIntervalTicks)
            {
                _lastSendTicks = now;
                _sendPending = false;
                SendNow();
            }
            else
                _sendPending = true;
        }

        // Always sends the value the gesture ended on, so a throttled write is never the last one.
        public void EndInteraction()
        {
            if (!_inInteraction) return;
            _inInteraction = false;

            if (_sendPending)
            {
                _sendPending = false;
                SendNow();
            }

            if (_before != null)
            {
                var before = _before;
                _before = null;
                Record(before, _value.ToModel());
            }
        }

        private void Record(IControlModel before, IControlModel after)
        {
            if (!_undoManager.IsApplying)
                _undoManager.Push(new ValueChangedCommand(_value, before, after));
        }

        private void SendNow() =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageValueChanged>(_value.ID, _value));
    }
}
