// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Prefabs;

namespace CMiX.Core.Presentations.Prefabs.Message
{
    public class MessageRemovePrefab : IMessagePrefab
    {
        public MessageRemovePrefab()
        {

        }

        public MessageRemovePrefab(Guid id, IPrefab prefab)
        {
            ID = id;
            PrefabID = prefab.ID;
        }

        public Guid ID { get; set; }
        public Guid PrefabID { get; set; }
    }
}
