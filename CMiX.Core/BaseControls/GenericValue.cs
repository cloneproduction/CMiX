// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ReceivableControl, IControl, IRecipient<IMessage>, IInteractiveValue
    {
        private static readonly long InteractionSendIntervalTicks = Stopwatch.Frequency / 60;

        public GenericValue()
        {

        }

        public GenericValue(ControlMessenger controlMessenger,
                            MessageFactory messageFactory,
                            ControlActivationService activationService,
                            UndoManager undoManager)
        {
            ID = Guid.NewGuid();
            MessageFactory = messageFactory;
            ControlMessenger = controlMessenger;
            UndoManager = undoManager;
            ResetCommand = new RelayCommand(Reset);
            IsActive = false;
            activationService.Register(this);
        }

        public void Reset()
        {
            Value = OriginalValue;
        }

        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public ControlMessenger ControlMessenger { get; set; }
        public MessageFactory MessageFactory { get; set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (IsActive)
                {
                    if (ValueInteraction.IsActive)
                    {
                        SetInteractionValue(value);
                        return;
                    }

                    var before = CaptureModel();
                    SetProperty(ref _value, value);
                    if (!IsReceiving && !(UndoManager?.IsApplying ?? false))
                    {
                        var after = CaptureModel();
                        UndoManager?.Push(new ValueChangedCommand(this, before, after));
                    }
                    SendValueChanged();
                }
                else
                    SetProperty(ref _value, value);
            }
        }

        private bool _inInteraction;
        private IControlModel _interactionBefore;
        private long _lastInteractionSendTicks;
        private bool _interactionSendPending;

        // Writes made while a ValueInteraction scope is open keep the local value and its change
        // notification per write, but capture the undo model once and rate limit the messages.
        private void SetInteractionValue(T value)
        {
            if (!_inInteraction)
            {
                _inInteraction = true;
                // Undo state is decided once per gesture, matching what the unthrottled path would
                // have recorded for the first write of the gesture.
                var recordsUndo = !IsReceiving
                                  && !(UndoManager?.IsApplying ?? false)
                                  && !(UndoManager?.IsSuppressed ?? false);
                _interactionBefore = recordsUndo ? CaptureModel() : null;
                _lastInteractionSendTicks = Stopwatch.GetTimestamp() - InteractionSendIntervalTicks;
                _interactionSendPending = false;
                ValueInteraction.Enlist(this);
            }

            // The name has to be explicit here because the caller member name would otherwise
            // become this helper instead of the property the bindings listen to.
            SetProperty(ref _value, value, nameof(Value));

            var now = Stopwatch.GetTimestamp();
            if (now - _lastInteractionSendTicks >= InteractionSendIntervalTicks)
            {
                _lastInteractionSendTicks = now;
                _interactionSendPending = false;
                SendValueChanged();
            }
            else
                _interactionSendPending = true;
        }

        // Always flushes the value the gesture ended on, so a throttled intermediate write can
        // never be the last thing the engine sees.
        public void EndInteraction()
        {
            if (!_inInteraction) return;
            _inInteraction = false;

            if (_interactionSendPending)
            {
                _interactionSendPending = false;
                SendValueChanged();
            }

            if (_interactionBefore != null)
            {
                var before = _interactionBefore;
                _interactionBefore = null;
                UndoManager?.Push(new ValueChangedCommand(this, before, CaptureModel()));
            }
        }

        private void SendValueChanged()
        {
            var message = MessageFactory.CreateMessage<MessageValueChanged>(this.ID, this);
            ControlMessenger.SendMessage(message);
        }

        private T _originalValue;
        public T OriginalValue
        {
            get => _originalValue;
            set => SetProperty(ref _originalValue, value);
        }

        protected virtual IControlModel CaptureModel() => ToModel();

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID) return;
            if (message is MessageValueChanged change)
                ReceiveWithoutEcho(() => Value = ((GenericValueModel<T>)change.Value).Value);
        }

        public IControlModel ToModel() => new GenericValueModel<T>
        {
            ID = ID,
            Value = Value
        };

        public void FromModel(IControlModel model)
        {
            var m = (GenericValueModel<T>)model;
            ID = m.ID;
            Value = m.Value;
        }
    }
}
