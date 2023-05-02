// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Networking.Messages
{
    public class MessageRequestControl : RequestMessage<object>
    {
        public MessageRequestControl(IMessage message)
        {
            ID = message.ID;
            Message = message;
        }

        public Guid ID { get; set; }
        public IMessage Message { get; set; }
    }
}
