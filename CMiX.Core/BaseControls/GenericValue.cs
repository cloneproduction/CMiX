// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ReceivableControl, IControl
    {
        public GenericValue(IValueSync sync, ControlRegistry registry)
        {
            _registry = registry;
            ID = Guid.NewGuid();
            _sync = sync;
            ResetCommand = new RelayCommand(Reset);
            registry.Register(this);
        }

        private readonly IValueSync _sync;
        private readonly ControlRegistry _registry;

        private Guid _id;
        public Guid ID
        {
            get => _id;
            set
            {
                var old = _id;
                _id = value;
                _registry.Rekey(this, old, value);
            }
        }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (!IsActive || IsSendingSuppressed)
                {
                    SetProperty(ref _value, value);
                    return;
                }

                // A write of the same value is a no-op: no undo step, no message.
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                // A momentary value is a pulse. It writes no undo step but it goes on the wire.
                if (IsMomentary)
                {
                    SetProperty(ref _value, value);
                    _sync.Send(this);
                    return;
                }

                _sync.Changing(this);
                SetProperty(ref _value, value);
                _sync.Changed(this);
            }
        }

        public bool IsMomentary { get; set; }

        private T _originalValue;
        public T OriginalValue
        {
            get => _originalValue;
            set => SetProperty(ref _originalValue, value);
        }

        // Takes the current value as the value that Reset restores.
        public override void SetDefaults() => OriginalValue = Value;

        public void Reset()
        {
            Value = OriginalValue;
        }

        public ICommand ResetCommand { get; }

        public override void Receive(IMessage message)
        {
            if (message is not MessageValueChanged change) return;
            using (SuppressSending())
                Value = ((GenericValueModel<T>)change.Value).Value;
        }

        public virtual IControlModel ToModel() => new GenericValueModel<T>
        {
            ID = ID,
            Value = Value
        };

        public virtual void FromModel(IControlModel model)
        {
            var m = (GenericValueModel<T>)model;
            ID = m.ID;
            Value = m.Value;
        }
    }
}
