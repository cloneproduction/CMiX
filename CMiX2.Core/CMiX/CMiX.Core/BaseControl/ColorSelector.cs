// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public class ColorSelector : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public ColorSelector(ColorSelectorModel colorSelectorModel)
        {
            ID = colorSelectorModel.ID;
            SelectedColor = (Color)ColorConverter.ConvertFromString(colorSelectorModel.SelectedColor);
            IsActive = true;
        }


        public Guid ID { get; set; }


        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
                if (IsActive)
                    ControlMessenger.Send<ColorSelectorModel>(this);
                Console.WriteLine(value);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
