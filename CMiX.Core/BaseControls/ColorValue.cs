// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class ColorValue : ObservableRecipient, IRecipient<MessageRequestControl>, IControl
    {
        //public ColorValue()
        //{
        //    ID = Guid.NewGuid();
        //    IsActive = true;
        //    SelectedColor = Color.FromArgb(255, 255, 0, 255);
        //}

        public ColorValue(Color color, ControlMessenger controlMessenger)
        {
            ID = Guid.NewGuid();
            IsActive = true;
            SelectedColor = color;
            ControlMessenger = controlMessenger;

        }

        ControlMessenger ControlMessenger { get; set; }
        public Guid ID { get; set; }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                SetProperty(ref _selectedColor, value);
                if (IsActive)
                    ControlMessenger.Send(this);
                Console.WriteLine(value);
            }
        }

        public void Receive(MessageRequestControl message)
        {
            ControlMessenger.Receive(this, message);
        }
    }
}
