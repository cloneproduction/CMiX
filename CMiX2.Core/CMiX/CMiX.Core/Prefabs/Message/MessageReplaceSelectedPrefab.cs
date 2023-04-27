// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefabs.Message
{
    public class MessageReplaceSelectedPrefab : IMessagePrefab
    {
        public MessageReplaceSelectedPrefab()
        {

        }

        public MessageReplaceSelectedPrefab(Guid id, Guid selectedPrefabID, Guid newPrefabID)
        {
            ID = id;
            SelectedPrefabID = selectedPrefabID;
            NewPrefabID = newPrefabID;
        }

        public Guid ID { get; set; }
        public Guid SelectedPrefabID { get; set; }
        public Guid NewPrefabID { get; set; }
    }
}
