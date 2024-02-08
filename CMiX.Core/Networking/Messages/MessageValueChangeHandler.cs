// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Networking.Messages
{
    public class MessageValueChangeHandler<T> : IMessageHandler
    {
        public MessageValueChangeHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            ((GenericValue<T>)control).Value = (T)((MessageValueChange)message).Value;
            return true;
        }
    }
}
