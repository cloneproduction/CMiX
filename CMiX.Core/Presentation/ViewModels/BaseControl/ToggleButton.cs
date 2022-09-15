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
    public class ToggleButton : ObservableRecipient, IControl, IRecipient<MessageRequestControl>
    {
        public ToggleButton(ToggleButtonModel toggleButtonModel)
        {
            this.ID = toggleButtonModel.ID;
            IsChecked = toggleButtonModel.IsChecked;

            IsActive = true;
        }

        public Guid ID { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetProperty(ref _isChecked, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }


        public IModel GetModel()
        {
            ToggleButtonModel model = new ToggleButtonModel();
            model.ID = this.ID;
            model.IsChecked = this.IsChecked;
            return model;
        }

        public void SetViewModel(IModel model)
        {
            ToggleButtonModel comboBoxModel = model as ToggleButtonModel;
            this.ID = comboBoxModel.ID;
            this.IsChecked = comboBoxModel.IsChecked;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
                message.Reply(this);
        }
    }
}
