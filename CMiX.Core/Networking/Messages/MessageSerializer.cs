// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking.Messages
{
    public class MessageSerializer
    {
        public MessageSerializer(CerasSerializer cerasSerializer)
        {
            Serializer = cerasSerializer;
        }

        public CerasSerializer Serializer { get; }

        public void ProcessMessage(byte[] data)
        {
            IMessage message = Serializer.Deserialize<IMessage>(data);
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
