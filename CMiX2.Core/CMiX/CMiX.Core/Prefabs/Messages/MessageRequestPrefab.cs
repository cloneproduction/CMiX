// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageRequestPrefab : RequestMessage<IPrefab>
    {

        public MessageRequestPrefab(Guid id)
        {
            ID = id;
        }

        public Guid ID { get; set; }
    }
}
