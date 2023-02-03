// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageAddPrefabToContainer : IMessagePrefab
    {
        public MessageAddPrefabToContainer()
        {

        }
        public MessageAddPrefabToContainer(Guid id, IPrefabModel container)
        {
            ID = id;
            ContainerID = container.ID;
            Model = container;
        }

        public Guid ID { get; set; }
        public Guid ContainerID { get; set; }
        public IPrefabModel Model { get; set; }
    }
}
