// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Slider : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public Slider(string name, SliderModel sliderModel)
        {
            Name = name;
            this.IsActive = false;
            this.ID = sliderModel.ID;
            this.Amount = sliderModel.Amount;

            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            this.IsActive = true;

            //CanSend = true;

            AddCommand = new RelayCommand(Add);
            SubCommand = new RelayCommand(Sub);
            ResetCommand = new RelayCommand(Reset);
        }


        public Guid ID { get; set; }
        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand MouseDownCommand { get; }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                SetProperty(ref _amount, value);
                Send(new MessageUpdateViewModel(this));
            }
        }

        private double _minimum = 0.0;
        public double Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }

        private double _maximum = 1.0;
        public double Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }

        private bool CanSend = false;

        public void Send(IMessage message)
        {
            if(CanSend)
                WeakReferenceMessenger.Default.Send<IMessage, int>(message, MessageType.Out);
        }

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
            }
        }


        private void Add() => Amount = Amount >= Maximum ? Maximum : Amount += 0.01;
        private void Sub() => Amount = Amount <= Minimum ? Minimum : Amount -= 0.01;
        public void Reset() => Amount = 0.0;


        public void SetViewModel(IModel model)
        {
            SliderModel sliderModel = model as SliderModel;
            this.ID = sliderModel.ID;
            this.Amount = sliderModel.Amount;
            Console.WriteLine("Slider SetViewModel Amount " + Amount);
        }

        public IModel GetModel()
        {
            SliderModel model = new SliderModel();
            model.ID = this.ID;
            model.Amount = this.Amount;
            return model;
        }
    }
}
