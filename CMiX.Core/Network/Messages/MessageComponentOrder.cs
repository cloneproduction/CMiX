// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Network.Messages
{
    public class MessageComponentOrder : IMessage
    {
        public MessageComponentOrder()
        {

        }

        public MessageComponentOrder(Guid id, IList<Guid> ids)
        {
            ID = id;
            IDs = ids;
        }

        public Guid ID { get; set ; }
        public IList<Guid> IDs { get; set; }
    }
}
