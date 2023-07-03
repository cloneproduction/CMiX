// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefab.Messages
{
    public class MessageItemOrderChange : IMessage
    {
        public MessageItemOrderChange()
        {

        }

        public MessageItemOrderChange(Guid id, IList<Guid> ids)
        {
            ID = id;
            IDs = ids;
        }

        public Guid ID { get; set; }
        public IList<Guid> IDs { get; set; }
    }
}
