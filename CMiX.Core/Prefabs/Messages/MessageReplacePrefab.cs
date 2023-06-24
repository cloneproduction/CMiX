// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefab.Messages
{
    public class MessageReplacePrefab : IMessagePrefab
    {
        public MessageReplacePrefab()
        {

        }

        public MessageReplacePrefab(Guid id, Guid oldPrefabID, Guid newPrefabID)
        {
            ID = id;
            OldPrefabID = oldPrefabID;
            NewPrefabID = newPrefabID;
        }

        public Guid ID { get; set; }
        public Guid OldPrefabID { get; private set; }
        public Guid NewPrefabID { get; private set; }
    }
}
