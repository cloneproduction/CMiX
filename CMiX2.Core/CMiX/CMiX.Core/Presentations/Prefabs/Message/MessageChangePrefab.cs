// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Prefabs;

namespace CMiX.Core.Presentations.Prefabs.Message
{
    public class MessageChangePrefab : IMessagePrefab
    {
        public MessageChangePrefab()
        {

        }

        public MessageChangePrefab(Guid id, IPrefab prefab, string propertyName)
        {
            ID = id;
            PrefabID = prefab.ID;
            PropertyName = propertyName;
        }

        public Guid ID { get; set; }
        public Guid PrefabID { get; set; }
        public string PropertyName { get; internal set; }
    }
}
