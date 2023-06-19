// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefab.Messages
{
    public class MessageAddPrefab : IMessagePrefab
    {
        public MessageAddPrefab()
        {

        }

        public MessageAddPrefab(Guid id, IPrefabModel container)
        {
            ID = id;
            Model = container;
        }

        public Guid ID { get; set; }
        public IPrefabModel Model { get; set; }
    }
}
