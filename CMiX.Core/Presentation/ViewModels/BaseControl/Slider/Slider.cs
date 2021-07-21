// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Slider : ObservableRecipient, IControl
    {
        public Slider(string name, SliderModel sliderModel)
        {
            Name = name;

            this.ID = sliderModel.ID;
            this.Amount = sliderModel.Amount;
            this.IsActive = true;


            AddCommand = new RelayCommand(Add);
            SubCommand = new RelayCommand(Sub);
            ResetCommand = new RelayCommand(Reset);
        }

        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<Slider, Message, string>(this, "IN", (r, m) => r.Receive(m));
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            WeakReferenceMessenger.Default.Unregister<Message, string>(this, "IN");
        }

        public void Receive(Message m)
        {
            m.Process(this);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }

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
                WeakReferenceMessenger.Default.Send<IMessage, string>(new MessageUpdateViewModel(this), "OUT");
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


        private void Add() => Amount = Amount >= Maximum ? Maximum : Amount += 0.01;
        private void Sub() => Amount = Amount <= Minimum ? Minimum : Amount -= 0.01;
        public void Reset() => Amount = 0.0;


        public void SetViewModel(IModel model)
        {
            SliderModel sliderModel = model as SliderModel;
            this.ID = sliderModel.ID;
            this.Amount = sliderModel.Amount;
            System.Console.WriteLine("Slider SetViewModel Amount " + Amount);
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
