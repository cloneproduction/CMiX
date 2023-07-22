// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefab.Messages
{
    public class MessageSelectedItemChanged : IMessagePrefab
    {
        public MessageSelectedItemChanged()
        {

        }

        public MessageSelectedItemChanged(Guid id, Guid selectedPrefabID)
        {
            ID = id;
            SelectedPrefabID = selectedPrefabID;
        }

        public Guid ID { get; set; }
        public Guid SelectedPrefabID { get; set; }
    }
}
