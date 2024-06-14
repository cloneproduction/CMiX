// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking.Messages
{
    public class MessageProcessor
    {
        public MessageProcessor(CerasSerializer cerasSerializer, ControlRepository controlRepository)
        {
            Serializer = cerasSerializer;
            ControlRepository = controlRepository;
        }

        public ControlRepository ControlRepository { get; set; }
        public CerasSerializer Serializer { get; set; }

        public void ProcessMessage(byte[] data)
        {
            IMessage message = Serializer.Deserialize<IMessage>(data);
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}
