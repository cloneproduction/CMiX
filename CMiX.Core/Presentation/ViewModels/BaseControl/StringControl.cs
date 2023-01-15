// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.BaseControl
{
    public class StringValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public StringValue(StringValueModel stringValueModel)
        {
            ID = stringValueModel.ID;
            Value = stringValueModel.Value;
            IsActive = true;
        }

        public Guid ID { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
            }
        }

        public IModel GetModel()
        {
            StringValueModel stringValueModel = new StringValueModel();
            stringValueModel.ID = ID;
            stringValueModel.Value = Value;
            return stringValueModel;
        }

        public void SetViewModel(IModel model)
        {
            StringValueModel stringValueModel = model as StringValueModel;
            this.ID = stringValueModel.ID;
            this.Value = stringValueModel.Value;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
