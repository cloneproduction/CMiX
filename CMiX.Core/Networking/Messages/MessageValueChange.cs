// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Networking.Messages
{
    public class MessageValueChange : IMessage
    {
        public MessageValueChange()
        {

        }

        public MessageValueChange(IControlModel model)
        {
            Model = model;
            ID = model.ID;
        }

        public Guid ID { get; set; }
        public IControlModel Model { get; set; }
    }
}
