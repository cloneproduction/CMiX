// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageSelectPrefab : IMessagePrefab
    {
        public MessageSelectPrefab()
        {

        }

        public MessageSelectPrefab(Guid id, IPrefab prefab)
        {
            ID = id;
            if(prefab != null)
                PrefabID = prefab.ID;
        }

        public Guid ID { get; set; }
        public Guid PrefabID { get; set; }
    }
}
