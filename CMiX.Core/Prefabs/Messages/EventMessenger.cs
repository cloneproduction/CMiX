// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Messages
{
    public class EventMessenger
    {
        public EventMessenger(IMapper mapper)
        {
            Mapper = mapper;
            CanSend = true;
        }

        IMapper Mapper { get; }
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
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageOnClick(iD), MessageType.Out);
        }
    }
}
