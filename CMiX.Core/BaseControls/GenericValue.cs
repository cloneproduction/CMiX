// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Networking.Messages;
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

        public GenericValue(IValueSync sync, ControlActivationService activationService)
        {
            ID = Guid.NewGuid();
            _sync = sync;
            ResetCommand = new RelayCommand(Reset);
            IsActive = false;
            activationService.Register(this);
        }

        private readonly IValueSync _sync;

        public void Reset()
        {
            Value = OriginalValue;
        }

        // Takes the current value as the value that Reset restores.
        public override void SetDefaults() => OriginalValue = Value;

        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public bool IsMomentary { get; set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (IsActive && !IsSendingSuppressed)
                {
                    if (ValueInteraction.IsActive)
                    {
                        SetInteractionValue(value);
                        return;
                    }

                    // A write that doesn't actually change the value (e.g. a control re-publishing
                    // its own current value the first time its view is built) must stay a true
                    // no-op - otherwise it records a phantom undo entry and a redundant network
                    // message for nothing.
                    if (EqualityComparer<T>.Default.Equals(_value, value))
                        return;

                    // A momentary value is a pulse. It writes no undo step but it goes on the wire.
                    if (IsMomentary)
                    {
                        SetProperty(ref _value, value);
                        _sync.Send(this);
                        return;
                    }

                    var before = CaptureModel();
                    SetProperty(ref _value, value);
                    _sync.Commit(this, before, CaptureModel());
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
                _interactionBefore = IsMomentary ? null : CaptureModel();
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
                _sync.Send(this);
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
                _sync.Send(this);
            }

            if (_interactionBefore != null)
            {
                var before = _interactionBefore;
                _interactionBefore = null;
                _sync.Record(this, before, CaptureModel());
            }
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
            if (message is not MessageValueChanged change) return;
            using (SuppressSending())
                Value = ((GenericValueModel<T>)change.Value).Value;
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
