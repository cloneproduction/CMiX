// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking.Messages
{
    public class MessageEnvelope
    {
        public string SenderID { get; set; }
        public Guid MessageID { get; set; }
        public IMessage Payload { get; set; }
    }
}
