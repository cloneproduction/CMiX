// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class StringValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public StringValue(ControlMessenger controlMessenger)
        {
            ControlMessenger = controlMessenger;
            ID = Guid.NewGuid();
            IsActive = true;
        }

        ControlMessenger ControlMessenger { get; set; }
        public Guid ID { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if (IsActive)
                    ControlMessenger.Send(this);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
