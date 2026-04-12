// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ReceivableControl, IControl, IRecipient<IMessage>
    {
        public GenericValue()
        {

        }

        public GenericValue(ControlMessenger controlMessenger,
                            MessageFactory messageFactory,
                            ControlActivationService activationService)
        {
            ID = Guid.NewGuid();
            MessageFactory = messageFactory;
            ControlMessenger = controlMessenger;
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
                    UndoManager.Record(this);
                    SetProperty(ref _value, value);
                    UndoManager.Commit(this);
                    var message = MessageFactory.CreateMessage<MessageValueChanged>(this.ID, this);
                    ControlMessenger.SendMessage(message);
                }
                else
                {
                    SetProperty(ref _value, value);
                }
            }
        }

        private T _originalValue;
        public T OriginalValue
        {
            get => _originalValue;
            set => SetProperty(ref _originalValue, value);
        }

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
            IsActive = false;
            Value = m.Value;
            IsActive = true;
        }
    }
}
