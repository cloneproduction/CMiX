// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels.Prefab
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
            if(message is MessagePrefabContainerChanged msg)
            {
                var manager = control as IPrefabManager;
                if(manager != null)
                {
                    IPrefabContainer container = manager.GetPrefab(msg.ContainerID) as IPrefabContainer;
                    container.Prefab = CompositionService.GetPrefab(msg.PrefabID);
                    return true;
                }
            }
            return false;
        }
    }
}
