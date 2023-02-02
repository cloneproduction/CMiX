// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public partial class FloatValue : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {

        public FloatValue(FloatValueModel floatValueModel)
        {
            this.ID = floatValueModel.ID;
            this.Value = floatValueModel.Value;
            this.IsActive = true;
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

        public IModel GetModel()
        {
            FloatValueModel model = new FloatValueModel();
            model.ID = this.ID;
            model.Value = this.Value;
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
