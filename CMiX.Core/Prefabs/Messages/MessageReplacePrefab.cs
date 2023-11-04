// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageReplacePrefab : IMessagePrefab
    {
        public MessageReplacePrefab()
        {

        }

        public MessageReplacePrefab(Guid id, IControlModel controlModel, EmptyPrefab emptyPrefab)
        {
            ID = id;
            ControlModel = controlModel;
            emptyPrefabID = emptyPrefab.ID;
        }

        public Guid ID { get; set; }
        public IControlModel ControlModel { get; private set; }
        public Guid emptyPrefabID { get; private set; }
    }
}
