// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Networking.Servers;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Messages
{
    public class EventMessenger
    {
        public EventMessenger(IMapper mapper, ControlRepository controlRepository)
        {
            Mapper = mapper;
            ControlRepository = controlRepository;
            CanSend = true;
        }

        IMapper Mapper { get; }
        ControlRepository ControlRepository { get; }

        private bool CanSend;

        public void Receive(Button button, IMessage message)
        {
            if (button.ID != message.ID)
                return;

            CanSend = false;
            button.OnClick();
            CanSend = true;

            Console.WriteLine("ButtonClick Handled ");
        }

        internal void SendMessageEvent(Guid iD)
        {
            foreach (Server server in ControlRepository.Servers)
                server.SendMessage(new MessageOnClick(iD));
        }
    }
}
