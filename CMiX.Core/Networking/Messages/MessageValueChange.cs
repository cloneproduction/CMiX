// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking.Messages
{
    public class MessageValueChange : IMessage 
    {
        public MessageValueChange()
        {

        }

        public MessageValueChange(Guid id, object value)
        {
            ID = id;
            Value = value;
        }

        public object Value { get; set; }
        public Guid ID { get; set; }
    }
}
