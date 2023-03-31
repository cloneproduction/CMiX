// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.Service;

namespace CMiX.Core.Presentations.Prefabs.Message
{
    public class MessagePrefabContainerChangedHandler : IMessageHandler
    {
        public MessagePrefabContainerChangedHandler(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        public CompositionService CompositionService { get; set; }

        public bool Handle(IControl control, IMessage message)
        {
            if (message is MessagePrefabContainerChanged msg)
            {
                var manager = control as IPrefabManager;
                if (manager != null)
                {
                    var container = manager.GetPrefab(msg.ContainerID) as IPrefabContainer;
                    container.Prefab = CompositionService.GetPrefab(msg.PrefabID);
                    return true;
                }
            }
            return false;
        }
    }
}
