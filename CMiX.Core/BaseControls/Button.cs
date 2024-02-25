// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class Button : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public Button()
        {
            
        }
        public Button(EventMessenger eventMessenger)
        {
            ID = Guid.NewGuid();
            EventMessenger = eventMessenger;
            ClickCommand = new RelayCommand(OnClick);
            IsActive = true;
        }

        public ICommand ClickCommand { get; set; }
        EventMessenger EventMessenger { get; set; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        public void OnClick()
        {
            Click?.Invoke(this, null);
            EventMessenger.SendMessageEvent(this.ID);

        }

        public Guid ID { get; set; }

        public void Receive(IMessage message)
        {
            EventMessenger.Receive(this, message);
        }
    }
}
