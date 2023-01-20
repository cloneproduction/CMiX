// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageAddPrefab : IMessagePrefab
    {
        public MessageAddPrefab()
        {

        }
        public MessageAddPrefab(Guid id, IPrefabContainer container)
        {
            ID = id;
            ContainerID = container.ID;
            Model = container.Prefab?.GetModel() as IPrefabModel;
        }

        public Guid ID { get; set; }
        public Guid ContainerID { get; set; }
        public IPrefabModel Model { get; set; }
    }
}
