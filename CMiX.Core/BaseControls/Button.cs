// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.BaseControls
{
    public class Button : ObservableRecipient, IControl//, IRecipient<MessageRequestControl>
    {
        public Button(ControlMessenger controlMessenger)
        {
            ControlMessenger = controlMessenger;
            ClickCommand = new RelayCommand(OnClick);
            IsActive = true;
        }

        public ICommand ClickCommand { get; set; }
        ControlMessenger ControlMessenger { get; set; }

        public delegate void ClickEventHandler(object source, EventArgs args);
        public event ClickEventHandler Click;

        public void OnClick()
        {
            Click?.Invoke(this, null);
            WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageOnClick(ID), MessageType.Out);
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        //public void Receive(MessageRequestControl message)
        //{
        //    //ControlMessenger.Receive(this, message);
        //}
    }
}
