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

        public IMessage CreateMessage<T>(Guid id, IControl control)
        {
            var type = typeof(T);

            if (type == typeof(MessageValueChange))
                return new MessageValueChange(id, Mapper.Map<IControlModel>(control));

            if (type == typeof(MessageAddItem))
                return new MessageAddItem(id, Mapper.Map<IControlModel>(control));

            if (type == typeof(MessageRemoveItem))
                return new MessageRemoveItem(id, control);

            return null;
        }

        public IMessage CreateMessage<T>(Guid id, IControl control, int index)
        {
            var type = typeof(T);

            if (type == typeof(MessageSelectedItemChanged))
                return new MessageSelectedItemChanged(id, Mapper.Map<IControlModel>(control), index);

            if (type == typeof(MessageReplaceItem))
                return new MessageReplaceItem(id, Mapper.Map<IControlModel>(control), index);

            return null;
        }

        public IMessage CreateMessage<T>(Guid id, int sourceIndex, int targetIndex)
        {
            var type = typeof(T);

            if (type == typeof(MessageMoveItem))
                return new MessageMoveItem(id, sourceIndex, targetIndex);

            return null;
        }

        public IMessage CreateMessage<T>(Guid id)
        {
            var type = typeof(T);

            if (type == typeof(MessageRemoveSelectedItem))
                return new MessageRemoveSelectedItem(id);

            return null;
        }
    }
}
