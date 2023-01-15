// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Integer2 : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public Integer2(Integer2Model integer2Model)
        {
            ID = integer2Model.ID;
            X = integer2Model.X;
            Y = integer2Model.Y;
            IsActive = true;
        }

        public ICommand AddCommand { get; }
        public ICommand SubCommand { get; }

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                SetProperty(ref _x, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
            }
        }

        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                SetProperty(ref _y, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this.GetModel()), MessageType.Out);
            }
        }


        public Guid ID { get; set; }


        public void SetViewModel(IModel model)
        {
            Integer2Model counterModel = model as Integer2Model;
            this.ID = counterModel.ID;
            this.X = counterModel.X;
            this.Y = counterModel.Y;
        }

        public IModel GetModel()
        {
            Integer2Model model = new Integer2Model();
            model.ID = this.ID;
            model.X = this.X;
            model.Y = this.Y;
            return model;
        }


        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)// && !message.HasReceivedResponse)
                message.Reply(this);
        }
    }
}
