// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public GenericValue(GenericValueModel<T> genericValueModel)
        {
            ID = genericValueModel.ID;
            Value = genericValueModel.Value;
            IsActive = true;
        }

        public Guid ID { get; set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if (IsActive)
                    ControlMessenger.Send<GenericValueModel<T>>(this);
                Console.WriteLine(value);
            }
        }

        public IModel GetModel()
        {
            var model = new GenericValueModel<T>(Value);
            model.ID = ID;
            model.Value = Value;
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
