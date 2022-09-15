// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Windows.Media;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorSelector : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public ColorSelector(ColorSelectorModel colorSelectorModel)
        {
            this.ID = colorSelectorModel.ID;
            SelectedColor = (Color)ColorConverter.ConvertFromString(colorSelectorModel.SelectedColor);

            this.IsActive = true;
        }

        public Guid ID { get; set; }

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

        public void SetViewModel(IModel model)
        {
            ColorSelectorModel colorSelectorModel = model as ColorSelectorModel;
            this.ID = colorSelectorModel.ID;
            this.SelectedColor = (Color)ColorConverter.ConvertFromString(colorSelectorModel.SelectedColor);
        }

        public IModel GetModel()
        {
            ColorSelectorModel model = new ColorSelectorModel();
            model.ID = this.ID;
            model.SelectedColor = this.SelectedColor.ToString();
            return model;
        }

        public void Receive(MessageRequestControl message)
        {
            if (message.ID == this.ID)
            {
                if (!message.HasReceivedResponse)
                    message.Reply(this);
            }
        }
    }
}
