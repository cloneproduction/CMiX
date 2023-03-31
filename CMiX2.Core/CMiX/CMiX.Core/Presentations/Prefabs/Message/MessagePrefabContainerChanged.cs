// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentations.Prefabs.Message
{
    public class MessagePrefabContainerChanged : IMessagePrefab
    {
        public MessagePrefabContainerChanged()
        {

        }

        public MessagePrefabContainerChanged(Guid id, Guid containerID, Guid prefabID)
        {
            ID = id;
            ContainerID = containerID;
            PrefabID = prefabID;
        }

        public Guid ID { get; set; }
        public Guid ContainerID { get; set; }
        public Guid PrefabID { get; set; }
    }
}
