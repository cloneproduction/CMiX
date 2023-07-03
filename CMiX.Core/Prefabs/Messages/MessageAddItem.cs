// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefab.Messages
{
    public class MessageAddItem : IMessagePrefab
    {
        public MessageAddItem()
        {

        }

        public MessageAddItem(Guid id, IControlModel controlModel)
        {
            ID = id;
            Model = controlModel;
        }

        public Guid ID { get; set; }
        public IControlModel Model { get; set; }
    }
}
