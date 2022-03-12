// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class BlendMode : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public BlendMode(BlendModeModel blendModeModel)
        {
            this.ID = blendModeModel.ID;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
            Mode = blendModeModel.Mode;
        }

        public Guid ID { get; set; }

        private string _mode;
        public string Mode
        {
            get { return _mode; }
            set
            {
                SetProperty(ref _mode, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }


        public void SetViewModel(IModel model)
        {
            BlendModeModel blendModeModel = model as BlendModeModel;
            this.ID = blendModeModel.ID;
            this.Mode = blendModeModel.Mode;
        }

        public IModel GetModel()
        {
            BlendModeModel model = new BlendModeModel();
            model.ID = this.ID;
            model.Mode = this.Mode;
            return model;
        }


        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if (message is MessageUpdateViewModel msg)
                    this.SetViewModel(msg.Model);
        }
    }
}
