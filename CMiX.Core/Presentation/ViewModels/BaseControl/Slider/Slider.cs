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
    //public class Slider : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    //{
    //    public Slider(string name, SliderModel sliderModel)
    //    {
    //        Name = name;
    //        this.ID = sliderModel.ID;
    //        this.Amount = sliderModel.Amount;

    //        //AddCommand = new RelayCommand(Add);
    //        //SubCommand = new RelayCommand(Sub);
    //        ResetCommand = new RelayCommand(Reset);

    //        this.IsActive = true;
    //    }


    //    public Guid ID { get; set; }
    //    public ICommand AddCommand { get; }
    //    public ICommand SubCommand { get; }
    //    public ICommand ResetCommand { get; }
    //    public ICommand MouseDownCommand { get; }


    //    private string _name;
    //    public string Name
    //    {
    //        get => _name;
    //        set => SetProperty(ref _name, value);
    //    }

    //    private float _amount;
    //    public float Amount
    //    {
    //        get => _amount;
    //        set
    //        {
    //            SetProperty(ref _amount, value);
    //            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
    //        }
    //    }

    //    private float _minimum = 0.0f;
    //    public float Minimum
    //    {
    //        get => _minimum;
    //        set => SetProperty(ref _minimum, value);
    //    }

    //    private float _maximum = 1.0f;
    //    public float Maximum
    //    {
    //        get => _maximum;
    //        set => SetProperty(ref _maximum, value);
    //    }


    //    //private void Add() => Amount = Amount >= Maximum ? Maximum : Amount += 0.01f;
    //    //private void Sub() => Amount = Amount <= Minimum ? Minimum : Amount -= 0.01f;
    //    public void Reset() => Amount = 0.0f;


    //    public void SetViewModel(IModel model)
    //    {
    //        SliderModel sliderModel = model as SliderModel;
    //        this.ID = sliderModel.ID;
    //        this.Amount = sliderModel.Amount;
    //        Console.WriteLine("Slider SetViewModel Amount " + Amount);
    //    }

    //    public IModel GetModel()
    //    {
    //        SliderModel model = new SliderModel();
    //        model.ID = this.ID;
    //        model.Amount = this.Amount;
    //        return model;
    //    }

    //    public void Receive(MessageRequestControl message)
    //    {
    //        if (message.ID == this.ID && !message.HasReceivedResponse)
    //            message.Reply(this);
    //    }
    //}
}
