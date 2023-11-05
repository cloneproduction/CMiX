// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Messages
{
    public class ManagerMessenger
    {
        public ManagerMessenger(IMapper mapper)
        {
            Mapper = mapper;
            CanSend = true;
        }

        IMapper Mapper { get; }

        private bool CanSend;

        public void Receive(IControl iDObject, MessageRequestControl message)
        {
            CanSend = false;

            if (message.ID == iDObject.ID && !message.HasReceivedResponse)
                message.Reply(iDObject);

            CanSend = true;
        }

        void Send(IMessage message)
        {
            WeakReferenceMessenger.Default.Send(message, MessageType.Out);
        }

        public void SendAddItem(Guid id, IControlModel prefab)
        {
            var message = new MessageAddItem(id, Mapper.Map<IControlModel>(prefab));
            Send(message);
        }

        public void SendAddItem(Guid id, IPrefab prefab)
        {
            var message = new MessageAddItem(id, Mapper.Map<IPrefabModel>(prefab));
            Send(message);
        }

        public void SendSelectedItemChanged(Guid id, IPrefab prefab, int index)
        {
            var message = new MessageSelectedItemChanged(id, prefab.ID, index);
            Send(message);
        }

        internal void SendReplaceEmptyPrefab(Guid id, IPrefab prefab, EmptyPrefab emptyPrefab)
        {
            var message = new MessageReplacePrefab(id, Mapper.Map<IPrefabModel>(prefab), emptyPrefab);
            Send(message);
        }
    }
}
