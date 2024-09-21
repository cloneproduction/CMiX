// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageFactory
    {
        public MessageFactory(IMapper mapper)
        {
            Mapper = mapper;
        }

        IMapper Mapper;

        public IMessage CreateMessage(Type messageType, Guid id, IControl control)
        {
            
            if (messageType == typeof(MessageValueChange))
                return new MessageValueChange(id, Mapper.Map<IControlModel>(control));

            if (messageType == typeof(MessageAddItem))
                return new MessageAddItem(id, Mapper.Map<IControlModel>(control));

            if (messageType == typeof(MessageRemoveItem))
                return new MessageRemoveItem(id, control);

            return null;
        }

        public IMessage CreateMessage(Type messageType, Guid id, IControl control, int index)
        {
            if (messageType == typeof(MessageSelectedItemChanged))
                return new MessageSelectedItemChanged(id, Mapper.Map<IControlModel>(control), index);

            if (messageType == typeof(MessageReplaceItem))
                return new MessageReplaceItem(id, Mapper.Map<IControlModel>(control), index);

            return null;
        }

        public IMessage CreateMessage(Type messageType, Guid id, int sourceIndex, int targetIndex)
        {
            if (messageType == typeof(MessageMoveItem))
                return new MessageMoveItem(id, sourceIndex, targetIndex);

            return null;
        }

        public IMessage CreateMessage(Type messageType, Guid id)
        {
            if (messageType == typeof(MessageRemoveSelectedItem))
                return new MessageRemoveSelectedItem(id);

            return null;
        }
    }
}
