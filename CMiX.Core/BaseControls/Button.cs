// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class Button : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public Button(ControlMessenger controlMessenger)
        {
            ID = Guid.NewGuid();
            ControlMessenger = controlMessenger;
            ClickCommand = new RelayCommand(OnClick);
            IsActive = true;
        }

        public ICommand ClickCommand { get; set; }
        ControlMessenger ControlMessenger { get; set; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        public void OnClick()
        {
            Click?.Invoke(this, null);
            var message = new MessageOnClick(this.ID);
            ControlMessenger.SendMessage(message);

        }

        public Guid ID { get; set; }

        public void Receive(IMessage message)
        {
            if (this.ID != message.ID)
                return;

            this.OnClick();

            Console.WriteLine("ButtonClick Received");
        }
    }
}
