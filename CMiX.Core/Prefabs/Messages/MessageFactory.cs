// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mapping;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageFactory
    {
        public MessageFactory(Mapper mapper)
        {
            Mapper = mapper;
        }

        public Mapper Mapper { get; set; }

        public IMessage CreateMessage<T>(Guid id, IControl control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            var model = Mapper.MapToModel(control);
            if (typeof(T) == typeof(MessageValueChanged))
                return new MessageValueChanged(id, model);
            else if (typeof(T) == typeof(MessageAddItem))
                return new MessageAddItem(id, model);
            else if (typeof(T) == typeof(MessageRemoveItem))
                return new MessageRemoveItem(id, control.ID);
            else
                return null;
        }

        public IMessage CreateMessage<T>(Guid id, IControl control, int index)
        {
            IControlModel model = null;

            if (control != null)
                model = Mapper.MapToModel(control);

            if (typeof(T) == typeof(MessageSelectedItemChanged))
                return new MessageSelectedItemChanged(id, model, index);
            else if (typeof(T) == typeof(MessageReplaceItem))
                return new MessageReplaceItem(id, model, index);
            else
                return null;
        }

        public IMessage CreateMessage<T>(Guid id, int sourceIndex, int targetIndex)
        {
            if (typeof(T) == typeof(MessageMoveItem))
                return new MessageMoveItem(id, sourceIndex, targetIndex);
            else
                return null;
        }

        public IMessage CreateMessage<T>(Guid id)
        {
            if (typeof(T) == typeof(MessageRemoveSelectedItem))
                return new MessageRemoveSelectedItem(id);
            else
                return null;
        }
    }
}
