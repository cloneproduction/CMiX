// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class IntegerValue : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public IntegerValue(IntegerValueModel counterModel)
        {
            ID = counterModel.ID;
            Value = counterModel.Value;

            AddCommand = new RelayCommand(Add);
            SubCommand = new RelayCommand(Sub);

            IsActive = true;
        }


        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                if(IsActive)
                    ControlMessenger.Send<IntegerValueModel>(this);
            }
        }

        public Guid ID { get; set; }

        private void Add() => Value += 1;

        private void Sub()
        {
            if (Value > 1)
                Value -= 1;
        }

        public IModel GetModel()
        {
            IntegerValueModel model = new IntegerValueModel();
            model.ID = this.ID;
            model.Value = this.Value;
            return model;
        }


        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
