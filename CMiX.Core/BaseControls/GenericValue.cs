// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class GenericValue<T> : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public GenericValue()
        {
            
        }

        public GenericValue(ControlMessenger controlMessenger)
        {
            ID = Guid.NewGuid();
            ControlMessenger = controlMessenger;
            ResetCommand = new RelayCommand(Reset);
            IsActive = true;
        }

        private void Reset()
        {
            Value = OriginalValue;
        }

        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public ControlMessenger ControlMessenger { get; set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if (IsActive)
                    ControlMessenger.Send(this);
                Console.WriteLine(value);
            }
        }

        private T _originalValue;
        public T OriginalValue
        {
            get => _originalValue;
            set => SetProperty(ref _originalValue, value);
        }

        public void Receive(IMessage message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
