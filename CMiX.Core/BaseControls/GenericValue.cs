// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ReceivableControl, IControl, IRecipient<IMessage>
    {
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
                    // A write of the same value is a no-op: no undo step, no message.
                    if (EqualityComparer<T>.Default.Equals(_value, value))
                        return;

                    // A momentary value is a pulse. It writes no undo step but it goes on the wire.
                    var before = IsMomentary ? null : CaptureModel();
                    SetProperty(ref _value, value);
                    _sync.Changed(this, before, CaptureModel());
                }
                else
                    SetProperty(ref _value, value);
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
