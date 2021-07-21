// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ComboBox<T> : ObservableRecipient, IControl
    {
        public ComboBox(ComboBoxModel<T> comboBoxModel)
        {
            this.ID = comboBoxModel.ID;
        }


        public Guid ID { get; set; }

        private T _selection;
        public T Selection
        {
            get => _selection;
            set
            {
                SetProperty(ref _selection, value);
                Messenger.Send<IMessage, string>(new MessageUpdateViewModel(this), "OUT");
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
    }
}
