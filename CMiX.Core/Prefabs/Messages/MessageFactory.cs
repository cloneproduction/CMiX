// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageFactory
    {
        public MessageFactory()
        {

        }


        private static readonly Dictionary<Type, Func<Guid, IControlModel, IMessage>> _factories = new()
        {
            [typeof(MessageValueChanged)] = (id, model) => new MessageValueChanged(id, model),
        };

        private static readonly Dictionary<Type, Func<Guid, IControlModel, int, IMessage>> _factoriesWithIndex = new()
        {
            [typeof(MessageSelectedItemChanged)] = (id, model, index) => new MessageSelectedItemChanged(id, model?.ID ?? Guid.Empty, index),
            [typeof(MessageAddItem)] = (id, model, index) => new MessageAddItem(id, model, index),
            [typeof(MessageRemoveItem)] = (id, model, index) => new MessageRemoveItem(id, model.ID, index),
        };

        public IMessage CreateMessage<T>(Guid id, IControl control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (!_factories.TryGetValue(typeof(T), out var factory))
                throw new NotSupportedException($"CreateMessage does not support type {typeof(T).Name}");
            return factory(id, control.ToModel());
        }

        public IMessage CreateMessage<T>(Guid id, IControl control, int index)
        {
            IControlModel model = control?.ToModel();
            if (!_factoriesWithIndex.TryGetValue(typeof(T), out var factory))
                throw new NotSupportedException($"CreateMessage does not support type {typeof(T).Name}");
            return factory(id, model, index);
        }

        public IMessage CreateMessage<T>(Guid id, int oldIndex, int newIndex)
        {
            if (typeof(T) == typeof(MessageMoveItem))
                return new MessageMoveItem(id, oldIndex, newIndex);
            throw new NotSupportedException($"CreateMessage does not support type {typeof(T).Name}");
        }

        public IMessage CreateMessage<T>(Guid id)
        {
            if (typeof(T) == typeof(MessageRemoveSelectedItem))
                return new MessageRemoveSelectedItem(id);
            throw new NotSupportedException($"CreateMessage does not support type {typeof(T).Name}");
        }

        public IMessage CreateMessage<T>(Guid id, IControlModel model, int index)
        {
            if (!_factoriesWithIndex.TryGetValue(typeof(T), out var factory))
                throw new NotSupportedException($"CreateMessage does not support type {typeof(T).Name}");
            return factory(id, model, index);
        }
    }
}
