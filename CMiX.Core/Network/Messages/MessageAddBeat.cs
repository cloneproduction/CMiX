// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddBeat : IMessage
    {
        public MessageAddBeat()
        {

        }

        public MessageAddBeat(Guid id)
        {
            ID = id;
        }
        public Guid ID { get; set; }
    }
}
