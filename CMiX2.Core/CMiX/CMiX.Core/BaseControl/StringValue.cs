// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels.BaseControl
{
    public class StringValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public StringValue(StringValueModel stringValueModel, CompositionService compositionService)
        {
            ID = stringValueModel.ID;
            Value = stringValueModel.Value;
            ControlMessenger = compositionService.ControlMessenger;
            IsActive = true;
        }

        private ControlMessenger ControlMessenger { get; set; }
        public Guid ID { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if(IsActive)
                    ControlMessenger.Send<StringValueModel>(this);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
