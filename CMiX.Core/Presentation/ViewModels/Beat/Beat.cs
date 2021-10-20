// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public abstract class Beat : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public Beat(BeatModel beatModel)
        {
            ResetCommand = new RelayCommand(Reset);
            MultiplyCommand = new RelayCommand(Multiply);
            DivideCommand = new RelayCommand(Divide);
        }

        public Guid ID { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand MultiplyCommand { get; set; }
        public ICommand DivideCommand { get; set; }
        public abstract double Period { get; set; }


        private AnimatedDouble _animatedDouble;
        public AnimatedDouble AnimatedDouble
        {
            get => _animatedDouble;
            set => SetProperty(ref _animatedDouble, value);
        }


        private double _multiplier;
        public virtual double Multiplier
        {
            get => _multiplier;
            set => SetProperty(ref _multiplier, value);
        }


        public void Reset() => Multiplier = 1;
        public abstract void Multiply();
        public abstract void Divide();

        public abstract void SetViewModel(IModel model);
        public abstract IModel GetModel();

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
                return;
            }
        }
    }
}
