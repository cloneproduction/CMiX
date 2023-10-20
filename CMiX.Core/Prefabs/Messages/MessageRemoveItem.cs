// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageRemoveItem : IMessagePrefab
    {
        public MessageRemoveItem()
        {

        }

        public MessageRemoveItem(Guid id, IControl control)
        {
            ID = id;
            Control = control.ID;
        }

        public Guid ID { get; set; }
        public Guid Control { get; set; }
    }
}
