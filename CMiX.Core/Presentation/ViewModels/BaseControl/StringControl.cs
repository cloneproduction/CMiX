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
    public class StringControl : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public StringControl(StringControlModel stringBaseModel)
        {
            ID = stringBaseModel.ID;
            Text = stringBaseModel.Text;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }

        public Guid ID { get; set; }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref _text, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }

        public IModel GetModel()
        {
            StringControlModel stringBaseModel = new StringControlModel();
            stringBaseModel.ID = ID;
            stringBaseModel.Text = Text;
            return stringBaseModel;
        }

        public void SetViewModel(IModel model)
        {
            StringControlModel stringBaseModel = model as StringControlModel;
            this.ID = stringBaseModel.ID;
            this.Text = stringBaseModel.Text;
        }

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if(message is MessageUpdateViewModel messageUpdateViewModel)
                this.SetViewModel(messageUpdateViewModel.Model);
        }
    }
}
