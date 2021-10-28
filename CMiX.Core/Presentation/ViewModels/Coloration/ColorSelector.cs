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
            ColorPicker = new ColorPicker(colorSelectorModel.ColorPickerModel);
            //this.SelectedColor = Utils.HexStringToColor(colorSelectorModel.ColorPickerModel.SelectedColor);
        }


        public Guid ID { get; set; }

        public ColorPicker ColorPicker { get; set; }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
            }
        }

        public bool? DialogResult => throw new NotImplementedException();

        public void SetViewModel(IModel model)
        {
            ColorSelectorModel colorSelectorModel = model as ColorSelectorModel;
            this.ID = colorSelectorModel.ID;
            this.ColorPicker.SetViewModel(colorSelectorModel.ColorPickerModel);
        }

        public IModel GetModel()
        {
            ColorSelectorModel model = new ColorSelectorModel();
            model.ID = this.ID;
            model.ColorPickerModel = (ColorPickerModel)ColorPicker.GetModel();
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
