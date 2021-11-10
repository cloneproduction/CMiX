// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Media;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorSelector : ObservableRecipient, IRecipient<IMessage>, IControl, IModalDialogViewModel
    {
        public ColorSelector(ColorSelectorModel colorSelectorModel)
        {
            this.ID = colorSelectorModel.ID;
            SelectedColor = new Color() { A = 255, R = 255, G = 0, B = 255 };
            //this.SelectedColor = Utils.HexStringToColor(colorSelectorModel.ColorPickerModel.SelectedColor);
        }


        public Guid ID { get; set; }


        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public bool? DialogResult { get; set; }

        public void SetViewModel(IModel model)
        {
            ColorSelectorModel colorSelectorModel = model as ColorSelectorModel;
            this.ID = colorSelectorModel.ID;
        }

        public IModel GetModel()
        {
            ColorSelectorModel model = new ColorSelectorModel();
            model.ID = this.ID;
            return model;
        }

        public void Receive(IMessage message)
        {
            if (message is MessageUpdateViewModel msg)
            {
                if (msg.ID == this.ID)
                    this.SetViewModel(msg.Model);
            }
        }
    }
}
