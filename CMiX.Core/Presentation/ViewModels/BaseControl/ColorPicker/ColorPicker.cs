// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Input;
using System.Windows.Media;
using CMiX.Core.Mathematics;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorPicker : ObservableRecipient, IRecipient<IMessage>, IControl
    {
        public ColorPicker(ColorPickerModel colorPickerModel)
        {
            SelectedColor = ColorExtensions.HexStringToColor(colorPickerModel.SelectedColor);
            WeakReferenceMessenger.Default.RegisterAll(this, MessageType.In);

            this.ID = colorPickerModel.ID;
            //Red = SelectedColor.R;
            //Green = SelectedColor.G;
            //Blue = SelectedColor.B;
            MouseDown = false;

            PreviewMouseDownCommand = new RelayCommand(PreviewMouseDown);
            PreviewMouseUpCommand = new RelayCommand(PreviewMouseUp);
            PreviewMouseLeaveCommand = new RelayCommand(PreviewMouseLeave);
        }


        public Guid ID { get; set; }
        public ICommand PreviewMouseDownCommand { get; set; }
        public ICommand PreviewMouseUpCommand { get; set; }
        public ICommand PreviewMouseLeaveCommand { get; set; }
        public bool MouseDown { get; set; }


        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(this), MessageType.Out);
            }
        }


        public void PreviewMouseDown()
        {
            MouseDown = true;
        }

        public void PreviewMouseUp()
        {
            MouseDown = false;
        }

        public void PreviewMouseLeave()
        {
            MouseDown = false;
        }


        public void SetViewModel(IModel model)
        {
            ColorPickerModel colorPickerModel = model as ColorPickerModel;
            this.ID = colorPickerModel.ID;
            this.SelectedColor = colorPickerModel.SelectedColor.HexStringToColor();
            Console.WriteLine("ColorPicker SetViewModel Color " + SelectedColor);
        }

        public IModel GetModel()
        {
            ColorPickerModel model = new ColorPickerModel();
            model.ID = this.ID;
            model.SelectedColor = this.SelectedColor.ColorToHexString();
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
