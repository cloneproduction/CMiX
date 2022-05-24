// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageRemovePrefab : IMessage
    {
        public MessageRemovePrefab()
        {

        }

        public MessageRemovePrefab(Guid managerID, IPrefab prefab)
        {

        }
        public Guid ID { get; set; }
    }
}
