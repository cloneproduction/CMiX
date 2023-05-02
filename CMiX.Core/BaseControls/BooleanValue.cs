// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class BooleanValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public BooleanValue(BooleanValueModel booleanValue)
        {
            ID = booleanValue.ID;
            Value = booleanValue.Value;
            IsActive = true;
        }

        public BooleanValue(bool value)
        {
            Value = value;
        }

        public Guid ID { get; set; }

        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if (IsActive)
                    ControlMessenger.Send<BooleanValueModel>(this);
                Console.WriteLine(value);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
