// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Network.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Network
{
    public static class ControlMessenger
    {
        public static bool CanSend = true;

        public static void Receive(IIDObject iDObject, MessageRequestControl message)
        {
            CanSend = false;

            if (message.ID == iDObject.ID && !message.HasReceivedResponse)
                message.Reply(iDObject);

            CanSend = true;
        }

        public static void Send(IControl control)
        {
            if (CanSend)
                WeakReferenceMessenger.Default.Send<IMessage, int>(new MessageUpdateViewModel(control.GetModel()), MessageType.Out);
        }
    }
}
