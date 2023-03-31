// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class FloatValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {

        public FloatValue(FloatValueModel floatValueModel)
        {
            ID = floatValueModel.ID;
            value = floatValueModel.Value;
            IsActive = true;
        }

        public Guid ID { get; set; }

        [ObservableProperty]
        private float value;

        partial void OnValueChanged(float value)
        {
            Console.WriteLine("FloatValueChanged = " + value);
            if (IsActive)
                ControlMessenger.Send<FloatValueModel>(this);
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
