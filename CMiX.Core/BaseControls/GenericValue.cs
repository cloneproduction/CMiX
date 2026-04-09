// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
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

        public GenericValue(ControlMessenger controlMessenger, MessageFactory messageFactory, ControlActivationService activationService)
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
                SetProperty(ref _value, value);
                if (IsActive)
                {
                    var message = MessageFactory.CreateMessage<MessageValueChanged>(this.ID, this);
                    ControlMessenger.SendMessage(message);
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
    }
}
