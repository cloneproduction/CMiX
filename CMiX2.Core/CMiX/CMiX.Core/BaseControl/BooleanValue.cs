// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public class BooleanValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public BooleanValue(BooleanValueModel booleanValue, CompositionService compositionService)
        {
            this.ID = booleanValue.ID;
            Value = booleanValue.Value;
            ControlMessenger = compositionService.ControlMessenger;
            IsActive = true;
        }

        public BooleanValue(bool value, CompositionService compositionService)
        {
            this.Value = value;
            this.ControlMessenger = compositionService.ControlMessenger;
        }

        public Guid ID { get; set; }
        private ControlMessenger ControlMessenger { get; set; }

        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if(IsActive)
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
