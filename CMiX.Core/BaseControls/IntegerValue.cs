// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class IntegerValue : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public IntegerValue(ControlMessenger controlMessenger)
        {
            IsActive = true;
            ControlMessenger = controlMessenger;
        }

        ControlMessenger ControlMessenger { get; set; }
        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }

        private int _value;
        public int Value
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

        public Guid ID { get; set; } = Guid.NewGuid();

        private void Add() => Value += 1;

        private void Sub()
        {
            if (Value > 1)
                Value -= 1;
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
