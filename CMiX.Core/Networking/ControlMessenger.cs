// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking
{
    public class ControlMessenger
    {
        public IMapper Mapper;
        public bool CanSend = true;

        public ControlMessenger(IMapper mapper)
        {
            Mapper = mapper;
        }

        public void Receive(IControl iDObject, MessageRequestControl message)
        {
            CanSend = false;

            if (message.ID == iDObject.ID && !message.HasReceivedResponse)
                message.Reply(iDObject);

            CanSend = true;
        }

        public void Send(IControl control)
        {
            if (CanSend)
            {
                var model = Mapper.Map<IControlModel>(control);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageValueChange(model), MessageType.Out);
            }     
        }
    }
}
