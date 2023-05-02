// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class FloatValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public FloatValue(FloatValueModel floatValueModel)
        {
            ID = floatValueModel.ID;
            Value = floatValueModel.Value;
            IsActive = true;
        }

        public Guid ID { get; set; }

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if (IsActive)
                    ControlMessenger.Send<FloatValueModel>(this);
                Console.WriteLine(value);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
