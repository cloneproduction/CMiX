// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Button : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public Button(ButtonModel buttonModel)
        {
            this.ID = buttonModel.ID;
            ClickCommand = new RelayCommand(OnClick);
            this.IsActive = true;
        }

        public ICommand ClickCommand { get; set; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        protected virtual void OnClick()
        {
            if (Click != null)
                Click(this, null);

            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
        }


        public Guid ID { get; set; }

        public IModel GetModel()
        {
            ButtonModel buttonModel = new ButtonModel();
            buttonModel.ID = ID;
            return buttonModel;
        }

        public void SetViewModel(IModel model)
        {
            ButtonModel buttonModel = model as ButtonModel;
            this.ID = buttonModel.ID;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
            {
                if (!message.HasReceivedResponse)
                    message.Reply(this);

                OnClick();
            }
        }
    }
}
