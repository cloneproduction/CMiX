// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;

namespace CMiX.Core.Networking
{
    public sealed class ValueSync : IValueSync, IInteractiveValue
    {
        private static readonly long SendIntervalTicks = Stopwatch.Frequency / 60;

        private readonly UndoManager _undoManager;
        private readonly MessageFactory _messageFactory;
        private readonly ControlMessenger _messenger;

        private IControl _value;
        private bool _inInteraction;
        private IControlModel _interactionBefore;
        private long _lastSendTicks;
        private bool _sendPending;

        public ValueSync(UndoManager undoManager, MessageFactory messageFactory, ControlMessenger messenger)
        {
            _undoManager = undoManager;
            _messageFactory = messageFactory;
            _messenger = messenger;
        }

        // The tests drive the clock through this.
        internal Func<long> Timestamp { get; set; } = Stopwatch.GetTimestamp;

        public void Changed(IControl value, IControlModel before, IControlModel after)
        {
            if (ValueInteraction.IsActive)
            {
                ChangedInInteraction(value, before);
                return;
            }

            if (before != null)
                Record(value, before, after);
            Send(value);
        }

        // A write in a ValueInteraction scope keeps the undo model once and rate limits the messages.
        private void ChangedInInteraction(IControl value, IControlModel before)
        {
            if (!_inInteraction)
            {
                _inInteraction = true;
                _value = value;
                _interactionBefore = before;
                _lastSendTicks = Timestamp() - SendIntervalTicks;
                _sendPending = false;
                ValueInteraction.Enlist(this);
            }

            var now = Timestamp();
            if (now - _lastSendTicks >= SendIntervalTicks)
            {
                _lastSendTicks = now;
                _sendPending = false;
                Send(value);
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
                Send(_value);
            }

            if (_interactionBefore != null)
            {
                var before = _interactionBefore;
                _interactionBefore = null;
                Record(_value, before, _value.ToModel());
            }
        }

        private void Record(IControl value, IControlModel before, IControlModel after)
        {
            if (!_undoManager.IsApplying)
                _undoManager.Push(new ValueChangedCommand(value, before, after));
        }

        private void Send(IControl value) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageValueChanged>(value.ID, value));
    }
}
