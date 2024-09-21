// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Messenger;

namespace CMiX.Core.Prefabs.Messages
{
    public class EventMessenger
    {
        public EventMessenger(ObservableCollection<Server> servers)
        {
            Servers = servers;
            CanSend = true;
        }

        ObservableCollection<Server> Servers { get; set; }

        private bool CanSend;

        public void Receive(Button button, IMessage message)
        {
            if (button.ID != message.ID)
                return;

            CanSend = false;
            button.OnClick();
            CanSend = true;

            Console.WriteLine("ButtonClick Received");
        }

        public void SendMessageEvent(Guid iD)
        {
            foreach (Server server in Servers)
                server.SendMessage(new MessageOnClick(iD));
        }
    }
}
