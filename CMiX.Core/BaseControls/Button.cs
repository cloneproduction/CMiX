// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class Button : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public Button()
        {
            IsActive = true;
        }

        public Button(ButtonModel buttonModel)
        {
            ID = buttonModel.ID;
            ClickCommand = new RelayCommand(OnClick);
            IsActive = true;
        }

        public ICommand ClickCommand { get; set; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        protected virtual void OnClick()
        {
            Click?.Invoke(this, null);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageValueChange(GetModel()), MessageType.Out);
        }


        public Guid ID { get; set; }

        public IControlModel GetModel()
        {
            var buttonModel = new ButtonModel();
            buttonModel.ID = ID;
            return buttonModel;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == ID)
            {
                if (!message.HasReceivedResponse)
                    message.Reply(this);

                OnClick();
            }
        }
    }
}
