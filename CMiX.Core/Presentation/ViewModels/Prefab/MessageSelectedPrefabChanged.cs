// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageSelectedPrefabChanged : IMessage
    {
        public MessageSelectedPrefabChanged()
        {

        }

        public MessageSelectedPrefabChanged(Guid id, IPrefab prefab)
        {
            ID = id;
            Model = prefab.GetModel();
        }

        public Guid ID { get; set; }
        public IModel Model { get; set; }
    }
}
