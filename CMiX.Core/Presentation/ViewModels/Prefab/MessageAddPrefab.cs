// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageAddPrefab : IMessage
    {
        public MessageAddPrefab()
        {

        }
        public MessageAddPrefab(Guid ManagerID, IPrefab prefab)
        {
            ID = ManagerID;
            PrefabModel = prefab.GetModel();
        }

        public Guid ID { get; set; }
        public IModel PrefabModel { get; internal set; }
    }
}
