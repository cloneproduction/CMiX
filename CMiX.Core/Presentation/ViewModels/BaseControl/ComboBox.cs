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
    public class GenericValue<T> : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public GenericValue(GenericValueModel<T> genericValueModel)
        {
            this.ID = genericValueModel.ID;
            this.Value = genericValueModel.Value;
            this.IsActive = true;
        }

        public Guid ID { get; set; }


        private T _value;
        public T Value
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
            GenericValueModel<T> model = new GenericValueModel<T>(this.Value);
            model.ID = this.ID;
            model.Value = this.Value;
            return model;
        }

        public void SetViewModel(IModel model)
        {
            GenericValueModel<T> genericValueModel = model as GenericValueModel<T>;
            this.ID = genericValueModel.ID;
            this.Value = genericValueModel.Value;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
