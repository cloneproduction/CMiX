// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Servers;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Prefabs.Messages
{
    public class ManagerMessenger
    {
        public ManagerMessenger(IMapper mapper, ServerRepository serverRepository)
        {
            Mapper = mapper;
            CanSend = true;
            ServerRepository = serverRepository;
            CollectionHandler = new MessageCollectionManagerHandler();
        }

        IMapper Mapper { get; }
        ServerRepository ServerRepository { get; }
        public MessageCollectionManagerHandler CollectionHandler { get; set; }

        private bool CanSend;

        public void Receive(PrefabManagerBase prefabManagerBase, IMessage messagePrefab)
        {
            if (messagePrefab is not IMessageManager messageManager)
                return;

            if (prefabManagerBase.ManagerData.ID != messagePrefab.ID)
                return;

            CanSend = false;
            CollectionHandler.Handle(prefabManagerBase, messagePrefab);
            CanSend = true;

            Console.WriteLine("Message " + messagePrefab.GetType().Name + " handled by ManagerMessenger");
        }

        void Send(IMessage message)
        {
            ServerRepository.GetServers().ForEach(x => x.SendMessage(message));
        }

        public void SendAddItem(Guid id, IControl control)
        {
            var message = new MessageAddItem(id, Mapper.Map<IControlModel>(control));
            Send(message);
        }

        public void SendSelectedItemChanged(Guid id, IControl control, int index)
        {
            Guid controlID = control?.ID ?? Guid.Empty;
            var message = new MessageSelectedItemChanged(id, controlID, index);
            Send(message);
        }

        public void SendReplaceEmptyPrefab(Guid id, IPrefab prefab, EmptyPrefab emptyPrefab)
        {
            var message = new MessageReplacePrefab(id, Mapper.Map<IControlModel>(prefab), emptyPrefab);
            Send(message);
        }

        public void SendMessageRemoveItem(Guid id, IControl control)
        {
            var message = new MessageRemoveItem(id, control);
            Send(message);
        }

        internal void SendMessageMoveItem(Guid id, int sourceIndex, int targetIndex)
        {
            var message = new MessageMoveItem(id, sourceIndex, targetIndex);
            Send(message);
        }
    }
}
