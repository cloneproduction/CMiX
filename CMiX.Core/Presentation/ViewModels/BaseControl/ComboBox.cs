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
    public class ComboBox<T> : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public ComboBox(ComboBoxModel<T> comboBoxModel)
        {
            this.ID = comboBoxModel.ID;
            this.Selection = comboBoxModel.Selection;
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);
        }


        public Guid ID { get; set; }


        private T _selection;
        public T Selection
        {
            get => _selection;
            set
            {
                SetProperty(ref _selection, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }


        public IModel GetModel()
        {
            ComboBoxModel<T> model = new ComboBoxModel<T>();
            model.ID = this.ID;
            model.Selection = this.Selection;
            return model;
        }

        public void SetViewModel(IModel model)
        {
            ComboBoxModel<T> comboBoxModel = model as ComboBoxModel<T>;
            this.ID = comboBoxModel.ID;
            this.Selection = comboBoxModel.Selection;
        }

        public void Receive(IMessage message)
        {
            if (message.ID != this.ID)
                return;

            if(message is MessageUpdateViewModel messageUpdateViewModel)
                this.SetViewModel(messageUpdateViewModel.Model);
        }
    }
}
