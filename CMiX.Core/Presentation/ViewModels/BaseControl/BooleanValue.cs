// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class BooleanValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public BooleanValue(BooleanValueModel booleanValue)
        {
            this.ID = booleanValue.ID;
            Value = booleanValue.Value;

            IsActive = true;
        }

        public Guid ID { get; set; }

        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                ControlMessenger.Send(this);
            }
        }


        public IModel GetModel()
        {
            BooleanValueModel model = new BooleanValueModel();
            model.ID = this.ID;
            model.Value = this.Value;
            return model;
        }

        public void SetViewModel(IModel model)
        {
            BooleanValueModel booleanValueModel = model as BooleanValueModel;
            this.ID = booleanValueModel.ID;
            this.Value = booleanValueModel.Value;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
