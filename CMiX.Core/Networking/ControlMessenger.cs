// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        public IMapper Mapper;
        public bool CanSend = true;

        public ControlMessenger(IMapper mapper, ServerRepository serverRepository)
        {
            Mapper = mapper;
            ServerRepository = serverRepository;
        }

        ServerRepository ServerRepository { get; set; }
        public void Receive<T>(GenericValue<T> control, IMessage message)
        {
            CanSend = false;

            if (message.ID != control.ID)
                return;
            
            if(message is MessageValueChange change)
                control.Value = (T)change.Value;

            CanSend = true;
        }

        public void Send<T>(GenericValue<T> control)
        {
            if (CanSend)
            {
                var message = new MessageValueChange(control.ID, control.Value);

                ServerRepository.GetServers().ForEach(x => x.SendMessage(message));

                Debug.WriteLine("Message Sent with Value : " + control.Value);
            }     
        }
    }
}
