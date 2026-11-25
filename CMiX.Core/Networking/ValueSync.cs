// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
        private readonly TimeProvider _time;
        private readonly long _sendIntervalTicks;

        private IControl _value;
        private IControlModel _before;
        private bool _inInteraction;
        private long _lastSendTicks;
        private bool _sendPending;
        private long _dragToken;

        public ValueSync(UndoManager undoManager, MessageFactory messageFactory, ControlMessenger messenger, TimeProvider time)
        {
            _undoManager = undoManager;
            _messageFactory = messageFactory;
            _messenger = messenger;
            _time = time;
            _sendIntervalTicks = time.TimestampFrequency / 60;
        }

        public IDisposable BeginInteraction()
        {
            if (_inInteraction) EndInteraction();
            _inInteraction = true;
            _lastSendTicks = _time.GetTimestamp() - _sendIntervalTicks;
            _sendPending = false;
            _before = null;
            _dragToken++;
            return new DragHandle(this, _dragToken);
        }

        // Ends the drag that opened it. A stale handle ends nothing.
        private sealed class DragHandle : IDisposable
        {
            private readonly ValueSync _owner;
            private readonly long _token;

            public DragHandle(ValueSync owner, long token)
            {
                _owner = owner;
                _token = token;
            }

            public void Dispose() => _owner.EndInteraction(_token);
        }

        public void Changing(IControl value)
        {
            _value = value;
            // In a drag the model before the first write is the before of the undo step.
            if (!_inInteraction || _before == null)
                _before = _value.ToModel();
        }

        public void Changed(IControl value)
        {
            _value = value;
            if (_inInteraction)
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
            if (_inInteraction)
            {
                _before = null;
                WriteInInteraction();
                return;
            }

            SendNow();
        }

        // A write during a drag rate limits the messages. The handle records the undo step at the end.
        private void WriteInInteraction()
        {
            var now = _time.GetTimestamp();
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
        private void EndInteraction()
        {
            if (!_inInteraction) return;
            _dragToken++;
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

        private void EndInteraction(long token)
        {
            if (token == _dragToken) EndInteraction();
        }

        private void Record(IControlModel before, IControlModel after) =>
            _undoManager.Push(new ValueChangedUndoCommand(_value, before, after));

        private void SendNow() =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageValueChanged>(_value.ID, _value));
    }
}
