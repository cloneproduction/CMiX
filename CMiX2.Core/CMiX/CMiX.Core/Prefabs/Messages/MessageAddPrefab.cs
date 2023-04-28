// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageAddPrefab : IMessagePrefab
    {
        public MessageAddPrefab()
        {

        }

        public MessageAddPrefab(Guid id, IPrefabModel container)
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
