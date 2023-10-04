// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking.Messages
{
    public class MessageOnClick : IMessage
    {
        public MessageOnClick()
        {

        }

        public MessageOnClick(Guid id)
        {
            ID = id;
        }

        public Guid ID { get; set; }
    }
}
