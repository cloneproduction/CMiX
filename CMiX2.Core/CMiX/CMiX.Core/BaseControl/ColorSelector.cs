// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.Network;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class ColorSelector : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        public ColorSelector(ColorSelectorModel colorSelectorModel)
        {
            this.ID = colorSelectorModel.ID;
            selectedColor = (Color)ColorConverter.ConvertFromString(colorSelectorModel.SelectedColor);

            this.IsActive = true;
        }


        public Guid ID { get; set; }

        [ObservableProperty]
        private Color selectedColor;


        partial void OnSelectedColorChanged(Color color)
        {
            Console.WriteLine("SelectedColorChanged = " + color);
            if (IsActive)
                ControlMessenger.Send<ColorSelectorModel>(this);
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
