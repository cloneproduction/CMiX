// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ToggleButton : ObservableRecipient, IControl
    {
        public ToggleButton(ToggleButtonModel toggleButtonModel)
        {
            this.ID = toggleButtonModel.ID;
        }


        public Guid ID { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetProperty(ref _isChecked, value);
                //Messenger.Send<IMessage, string>(new MessageUpdateViewModel(this), "OUT");
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
    }
}
